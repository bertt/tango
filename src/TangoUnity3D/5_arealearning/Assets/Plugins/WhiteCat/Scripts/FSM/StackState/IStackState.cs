
namespace WhiteCat.FSM
{
	/// <summary>
	/// 栈状态接口
	/// </summary>
	public interface IStackState : IState
	{
		/// <summary>
		/// 状态入栈时触发. 注意: 在此阶段修改状态机的状态是未定义的行为
		/// </summary>
		void OnPush();


		/// <summary>
		/// 状态出栈时触发. 注意: 在此阶段修改状态机的状态是未定义的行为
		/// </summary>
		void OnPop();

	} // interface IStackState

} // namespace WhiteCat.FSM