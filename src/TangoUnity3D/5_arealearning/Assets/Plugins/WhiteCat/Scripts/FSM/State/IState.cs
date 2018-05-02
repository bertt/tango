
namespace WhiteCat.FSM
{
	/// <summary>
	/// 状态接口
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// 进入状态时触发. 注意: 在此阶段修改状态机的状态是未定义的行为
		/// </summary>
		void OnEnter();

		/// <summary>
		/// 离开状态时触发. 注意: 在此阶段修改状态机的状态是未定义的行为
		/// </summary>
		void OnExit();

		/// <summary>
		/// 更新状态时触发
		/// </summary>
		void OnUpdate(float deltaTime);

	} // interface IState

} // namespace WhiteCat.FSM