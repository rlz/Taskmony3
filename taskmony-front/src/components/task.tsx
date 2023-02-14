import yes from "../images/checkbox-yes.svg";
import no from "../images/checkbox-no.svg";
import followBlue from "../images/followed.svg";
import followGray from "../images/follow.svg";
import divider from "../images/divider.svg";
import commentsI from "../images/comment2.svg";
import createdByI from "../images/by.svg";
// import recurrentI from "../images/recurrent.svg";
import recurrentI from "../images/arrows-rotate.svg";
import { useState } from "react";
import { EditedTask } from "./edited/edited-task";
import { useAppDispatch } from "../utils/hooks";
import { changeCompleteTaskDate, changeTaskFollowed, CHANGE_OPEN_TASK, CHANGE_TASKS, deleteTask, openTask, RESET_TASK } from "../services/actions/tasksAPI";
import { getCookie } from "../utils/cookies";
import { nowDate } from "../utils/APIUtils";

type TaskProps = {
  label: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: string;
  createdBy?: string;
  direction?: string;
};

export const Task = ({task,direction}) => {
  const myId = getCookie("id");
  const dispatch = useAppDispatch();
  const [edited,setEdited] = useState(false);
  const open = () => {
    console.log("opening");
    if(edited) return;
    setEdited(true);
    dispatch({type:CHANGE_OPEN_TASK,task:task});
  }
  const save = (task) => {
    if(!edited) return;
    dispatch({type:CHANGE_TASKS,task:task});
    dispatch({type:RESET_TASK})
    setEdited(false);
  }
  const deleteThisTask = (task) => {
  console.log(task)
  dispatch(deleteTask(task.id));
  dispatch({type:RESET_TASK})
  setEdited(false);
  }

  const isFollowed = () => {
   if(task.subscribers.some(s=>s.id==myId)) return true;
   return false;
  }
  const changeCheck = (markComplete) => {
    console.log("marking",markComplete);
    if (markComplete) dispatch(changeCompleteTaskDate(task.id,nowDate()))
    else dispatch(changeCompleteTaskDate(task.id,null))
  }
  const changeFollowed = (markFollowed) => {
    console.log("following",markFollowed);
    dispatch(changeTaskFollowed(task.id,markFollowed))
  }
 
  return (
  <div onClick={open}>
  {edited ? <EditedTask save={save} deleteTask={deleteThisTask} direction={direction} changeCheck={changeCheck}/> : <TaskUnedited 
  label={task.description}
  checked={!!task.completedAt}
  changeCheck={changeCheck}
  changeFollowed={changeFollowed}
  followed={direction || isFollowed()?isFollowed():undefined}
  direction={direction ? null : task.direction?.name}
  comments={task?.comments?.length}/>}
  </div>
  )
}

export const TaskUnedited = ({
  label,
  checked,
  followed,
  comments,
  recurrent,
  createdBy,
  direction,
  changeCheck,
  changeFollowed
}: TaskProps) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <img src={checked ? yes : no} onClick={(e)=>{e.stopPropagation();changeCheck(!checked)}}></img>
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
        {typeof followed !== "undefined" && (
          <img className="w-4" src={followed ? followBlue : followGray} onClick={(e)=>{e.stopPropagation();changeFollowed(!followed)}}></img>
        )}
      </div>
      <div className={"gap flex justify-start pb-2 w-full ml-1"}>
        {recurrent && (
          <TaskDetails icon={recurrentI} label={recurrent} hasBorder />
        )}
        {createdBy && (
          <TaskDetails icon={createdByI} label={`by ${createdBy}`} hasBorder />
        )}
        {
          <TaskDetails
            icon={commentsI}
            label={comments ? comments.toString() : "0"}
            hasBorder
          />
        }
        {<TaskDetails label={direction} textColor="text-yellow-500" />}
      </div>
    </div>
  );
};

type TaskDetailsProps = {
  icon?: string;
  label?: string;
  hasBorder?: boolean;
  textColor?: string;
};

export const TaskDetails = ({
  icon,
  label,
  hasBorder,
  textColor,
}: TaskDetailsProps) => {
  return (
    <div className={`flex flex-nowrap gap-1 mr-1  ${!icon ? "ml-5" : "ml-1"}`}>
      {icon && <img src={icon}></img>}
      <span
        className={
          "font-semibold inline whitespace-nowrap text-xs text-blue-500 mr-1 " +
          textColor
        }
      >
        {label}
      </span>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};
