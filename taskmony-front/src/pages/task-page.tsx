import backI from "../images/arrow-left.svg";
import arrowUp from "../images/arrow-up.svg";
import { useEffect, useRef, useState } from "react";
import arrowUpGray from "../images/arrow-up-gray.svg";
import useIsFirstRender, { useAppDispatch, useAppSelector } from "../utils/hooks";
import { useLocation, useNavigate } from "react-router-dom";
import { Description } from "../components/edited/edited-task/description";
import { About } from "../components/edited/edited-task/about";
import { Details } from "../components/edited/edited-task/task-details";
import { Comments } from "../components/edited/comments/comments";
import { sendTaskComment } from "../services/actions/comments";
import { CHANGE_OPEN_TASK } from "../services/actions/tasksAPI";

export const TaskPage = () => {
    const location = useLocation();
    const from = location.state?.from;
    const navigate = useNavigate();
 
  return (
    <>
      <img src={backI} className={`w-4 m-5 cursor-pointer ${!from?'invisible':''}`} onClick={() => {if(from) navigate(from)}}/>
      <TaskInfo save={undefined} />
    </>
  );
};

type TaskProps = {
  label?: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: boolean;
  createdBy?: string;
  direction?: string;
  save: Function;
  close?: Function;
  changeCheck?: Function;
  deleteTask?: Function;
};

const TaskInfo = ({
  direction,
  save,
  close,
  deleteTask,
  followed,
  recurrent,
}: TaskProps) => {
    const dispatch = useAppDispatch();
    const isFirst = useIsFirstRender();
    const taskId = location.pathname.split("/")[2];
    const tasks = useAppSelector((store) => store.tasks.items);
    const myTasks = tasks.filter(i=> i.id == taskId)
    if(myTasks[0]) dispatch({type: CHANGE_OPEN_TASK, task: myTasks[0]});
    const [description, setDescription] = useState(myTasks[0]?.description);
  const task = useAppSelector((store) => store.editedTask);
  const navigate = useNavigate();
  const closeBtn = useRef(null);
  const saveBtn = useRef(null);
  const onKeyPress = (event: any) => {
    if (event.key === "Escape") {
      console.log("Escape");
      if (task.id && saveBtn.current) saveBtn.current.click();
    }
    if (event.key === "Enter") {
      console.log("Enter");
      if (task.id && saveBtn.current) saveBtn.current.click();

    }
  };
  useEffect(() => {
    document.addEventListener("keydown", onKeyPress);
    return () => {
      document.removeEventListener("keydown", onKeyPress);
    };
  }, []);
  if(isFirst && task.id == "")
  return(
    <div className="flex items-center justify-center mt-8">
    </div>
  )
  if(myTasks && myTasks.length === 0 && !isFirst)
  return(
    <div className="flex items-center justify-center mt-8">
    <p className="font-semibold">You cannot access this task or it does not exist</p>
    </div>
  )
  return (
    <div className="m-4 editedTask rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <Description description={description} setDescription={setDescription} closeBtnRef={undefined} />
      </div>
      <About />
      <Details fromDirection={direction} />
      {task.id && (
        <Comments
          send={(input) => {
            dispatch(sendTaskComment(task.id, input));
          }}
          comments={task.comments}
        />
      )}
    </div>
  );
};
