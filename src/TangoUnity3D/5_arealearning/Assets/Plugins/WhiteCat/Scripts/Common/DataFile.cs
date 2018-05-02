using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace WhiteCat
{
	/// <summary>
	/// 支持同步或异步读写的数据文件
	/// 请继承 BinaryDataFile 或 TextDataFile, 添加数据并实现 Reset, OnRead 和 OnWrite
	/// </summary>
	public abstract class DataFile
	{
		/// <summary>
		/// 任务类别
		/// </summary>
		public enum TaskType
		{
			ReadAsync,							// 异步读
			WriteAsync,                         // 异步写
			ExitAsync,							// 退出
		}


		/// <summary>
		/// 任务内容
		/// </summary>
		public struct Task
		{
			public int index;					// 任务编号
			public TaskType type;               // 任务类别
			public bool success;				// 任务是否成功完成
			public DataFile dataFile;           // 对于读任务, 此对象存储读取的数据; 对于写任务, 此对象是请求时的数据副本
		}


		// 任务项
		struct TaskItem
		{
			public Task task;					// 任务
			public Action<Task> callback;		// 完成任务的回调
		}


		string _filePath;

		static Queue<TaskItem> _requestedTasks;	// 请求的任务队列
		static int _nextRequestedTaskIndex;     // 下一个请求的任务编号

		static Queue<TaskItem> _finishedTasks;  // 完成的任务队列
		static int _lastfinishedTaskIndex;      // 最后一个完成的任务编号

		static Semaphore _semaphore;
		static Thread _thread;


		// 初始化文件路径
		public DataFile(string filePath)
		{
			_filePath = filePath;
		}


		// 初始化任务队列和后台线程
		static DataFile()
		{
			_requestedTasks = new Queue<TaskItem>(4);
			_nextRequestedTaskIndex = 0;

			_finishedTasks = new Queue<TaskItem>(4);
			_lastfinishedTaskIndex = -1;

			_semaphore = new Semaphore(0, 1);

			_thread = new Thread(RunThread);
			_thread.IsBackground = true;
			_thread.Priority = ThreadPriority.Lowest;
			_thread.Start();
		}


		// 后台线程逻辑
		static void RunThread()
		{
			while (true)
			{
				_semaphore.WaitOne();

				TaskItem item;

				lock (_requestedTasks)
				{
					item = _requestedTasks.Peek();
				}

				switch (item.task.type)
				{
					case TaskType.ReadAsync:
						lock (item.task.dataFile)
						{
							item.task.success = item.task.dataFile.Read();
						}
						break;

					case TaskType.WriteAsync:
						item.task.success = item.task.dataFile.Write();
						break;

					case TaskType.ExitAsync:
						item.task.success = true;
						break;
				}

				lock (_requestedTasks)
				{
					lock (_finishedTasks)
					{
						_requestedTasks.Dequeue();
						_finishedTasks.Enqueue(item);

						if (item.task.type == TaskType.ExitAsync)
						{
							return;
						}
					}

					if (_requestedTasks.Count > 0)
					{
						SafeReleaseSemaphore();
					}
				}
			}
		}


		// 安全释放信号量
		static void SafeReleaseSemaphore()
		{
			try
			{
				_semaphore.Release();
			}
			catch
			{
			}
		}


		// 新建任务, 返回新任务的编号
		static int NewTask(TaskType taskType, DataFile dataFile, Action<Task> callback)
		{
			lock (_requestedTasks)
			{
				var item = new TaskItem();
				item.task.index = _nextRequestedTaskIndex++;
				item.task.type = taskType;
				item.task.dataFile = dataFile;
				item.callback = callback;

				_requestedTasks.Enqueue(item);
				SafeReleaseSemaphore();

				return item.task.index;
			}
		}


		/// <summary>
		/// 未完成的任务数量 (包含退出任务). 建议在游戏退出前检查所有任务是否已经完成
		/// </summary>
		public static int unfinishedTaskCount
		{
			get
			{
				lock (_requestedTasks)
				{
					lock (_finishedTasks)
					{
						return _requestedTasks.Count + _finishedTasks.Count;
					}
				}
			}
		}


		/// <summary>
		/// 判断一个任务是否已经完成
		/// </summary>
		public static bool IsTaskFinished(int taskIndex)
		{
			return taskIndex <= _lastfinishedTaskIndex;
		}


		/// <summary>
		/// 根据需要的频率在主线程中调用此方法
		/// </summary>
		public static void Update()
		{
			lock (_finishedTasks)
			{
				while (_finishedTasks.Count > 0)
				{
					var item = _finishedTasks.Dequeue();
					_lastfinishedTaskIndex = item.task.index;
					if (item.callback != null) item.callback(item.task);
				}
			}
		}


		public string filePath
		{
			get { return _filePath; }
		}


		/// <summary>
		/// 请求异步读文件
		/// 在读过程中, 需避免访问当前对象, 否则会造成主线程阻塞
		/// </summary>
		/// <returns> 任务编号 </returns>
		public int ReadAsync(Action<Task> onFinish = null)
		{
			return NewTask(TaskType.ReadAsync, this, onFinish);
		}


		/// <summary>
		/// 请求异步写文件
		/// </summary>
		/// <returns> 任务编号 </returns>
		public int WriteAsync(Action<Task> onFinish = null)
		{
			return NewTask(TaskType.WriteAsync, Clone(), onFinish);
		}


		/// <summary>
		/// 请求退出背景线程. 建议使用此方法并在确定任务完成后才退出游戏
		/// </summary>
		/// <returns> 任务编号 </returns>
		public static int ExitAsync(Action<Task> onFinish = null)
		{
			return NewTask(TaskType.ExitAsync, null, onFinish);
		}


		/// <summary>
		/// (同步) 读文件
		/// </summary>
		public abstract bool Read();


		/// <summary>
		/// (同步) 写文件
		/// </summary>
		public abstract bool Write();


		/// <summary>
		/// 重置数据. 当读取失败时会自动执行, 或在必要时可以手动执行
		/// </summary>
		public abstract void Reset();


		/// <summary>
		/// 返回对象的拷贝
		/// 在请求异步写文件时提供对象副本, 之后对副本对象执行异步写文件操作.
		/// 默认实现为对象内存的浅拷贝, 所有非静态的字段都会被直接复制
		/// </summary>
		/// <returns> 副本对象 </returns>
		public virtual DataFile Clone()
		{
			return MemberwiseClone() as DataFile;
		}

	} // class DataFile


	/// <summary>
	/// 二进制数据文件
	/// </summary>
	public abstract class BinaryDataFile : DataFile
	{
		/// <summary>
		/// (同步) 读文件
		/// </summary>
		public sealed override bool Read()
		{
			FileStream fs = null;
			bool success = true;

			try
			{
				fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				using (var reader = new BinaryReader(fs))
				{
					OnRead(reader);
				}
			}
			catch
			{
				success = false;
				Reset();
			}
			finally
			{
				if (fs != null)
				{
					fs.Dispose();
					fs.Close();
				}
			}

			return success;
		}


		/// <summary>
		/// (同步) 写文件
		/// </summary>
		public sealed override bool Write()
		{
			FileStream fs = null;
			bool success = true;

			try
			{
				fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				using (var writer = new BinaryWriter(fs))
				{
					OnWrite(writer);
				}
			}
			catch
			{
				success = false;
			}
			finally
			{
				if (fs != null)
				{
					fs.Dispose();
					fs.Close();
				}
			}

			return success;
		}


		/// <summary>
		/// 自定义读取过程
		/// 在调用同步读时, 此方法将在主线程执行; 在异步读请求后, 此方法将在背景线程执行
		/// 注意: 实现时无需在内部捕获异常, 并且仅可修改当前对象本身的数据
		/// </summary>
		protected abstract void OnRead(BinaryReader reader);


		/// <summary>
		/// 自定义写入过程
		/// 在调用同步写时, 此方法将在主线程执行; 在异步写请求后, 此方法将在背景线程执行
		/// 注意: 实现时无需在内部捕获异常, 并且仅可访问当前对象本身的数据
		/// </summary>
		protected abstract void OnWrite(BinaryWriter writer);


		/// <summary>
		/// 创建二进制数据文件
		/// </summary>
		public BinaryDataFile(string filePath) : base(filePath)
		{
		}

	} // class BinaryDataFile


	/// <summary>
	/// 文本数据文件
	/// </summary>
	public abstract class TextDataFile : DataFile
	{
		/// <summary>
		/// (同步) 读文件
		/// </summary>
		public sealed override bool Read()
		{
			FileStream fs = null;
			bool success = true;

			try
			{
				fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				using (var reader = new StreamReader(fs))
				{
					OnRead(reader);
				}
			}
			catch
			{
				success = false;
				Reset();
			}
			finally
			{
				if (fs != null)
				{
					fs.Dispose();
					fs.Close();
				}
			}

			return success;
		}


		/// <summary>
		/// (同步) 写文件
		/// </summary>
		public sealed override bool Write()
		{
			FileStream fs = null;
			bool success = true;

			try
			{
				fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				using (var writer = new StreamWriter(fs))
				{
					OnWrite(writer);
				}
			}
			catch
			{
				success = false;
			}
			finally
			{
				if (fs != null)
				{
					fs.Dispose();
					fs.Close();
				}
			}

			return success;
		}


		/// <summary>
		/// 自定义读取过程
		/// 在调用同步读时, 此方法将在主线程执行; 在异步读请求后, 此方法将在背景线程执行
		/// 注意: 实现时无需在内部捕获异常, 并且仅可修改当前对象本身的数据
		/// </summary>
		protected abstract void OnRead(StreamReader reader);


		/// <summary>
		/// 自定义写入过程
		/// 在调用同步写时, 此方法将在主线程执行; 在异步写请求后, 此方法将在背景线程执行
		/// 注意: 实现时无需在内部捕获异常, 并且仅可访问当前对象本身的数据
		/// </summary>
		protected abstract void OnWrite(StreamWriter writer);


		/// <summary>
		/// 创建文本数据文件
		/// </summary>
		public TextDataFile(string filePath) : base(filePath)
		{
		}

	} // class TextDataFile

} // namespace WhiteCat