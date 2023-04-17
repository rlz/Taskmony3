import { changeIdeaDescription, CHANGE_IDEA_DESCRIPTION } from "../../../services/actions/ideasAPI";
import { changeTaskDescription, CHANGE_TASK_DESCRIPTION } from "../../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";

export const Description = () => {
const dispatch = useAppDispatch();
const task = useAppSelector((store) => store.editedTask);
return(
  <input
  className={"bg-slate-500 bg-opacity-0 font-semibold text-sm focus:outline-none placeholder:font-thin placeholder:text-black decoration-slate-50"}
  id="description"
  autoFocus
  placeholder={"describe a task"}
  autoComplete="off"
  value={task.description}
  onKeyDown={(e)=>
    {if(e.key === "Enter" || e.key === "Escape") e.target.blur();}
  }
  onChange={(e) =>
    dispatch({
      type: CHANGE_TASK_DESCRIPTION,
      payload: e.target.value,
    })
  }
  onBlur={(e) => {
    if (task.id)
      dispatch(changeTaskDescription(task.id, e.target.value));
  }}
/>
)}