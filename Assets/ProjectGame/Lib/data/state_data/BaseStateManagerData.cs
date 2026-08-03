using System;
using System.Collections.Generic;
namespace GameCore.States.Managers
{
    public abstract class BaseStateManagerData<T> where T : Enum
    {

        protected T now_state_id = default;

        protected T old_state_id = default;
        
        public T SaveStateID { get; set; }

        protected List<T> stack_id_list = new List<T>();

        public void PushStateID(T state_id)
        {
            stack_id_list.Add(state_id);
        }

        public T PopStateID()
        {
            if (stack_id_list.Count > 0)
            {
                var state_id = stack_id_list[0];
                return state_id;
            }
            return default;
        }
        
        public void PopUpStateID()
        {
            if (stack_id_list.Count > 0)
            {
                stack_id_list.RemoveAt(0);

            }
        }


        public virtual void ChangeStateNowID(T new_state_id)
        {
            old_state_id = now_state_id;
            now_state_id = new_state_id;
        }

        public T GetNowStateID()
        {
            return now_state_id;
        }

        public T GetOldStateID()
        {
            return old_state_id;
        }

    }
}
