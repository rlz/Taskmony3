import { useEffect, useState } from "react";
import { OpenTask } from "./open-task/open-task";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import {
  changeCompleteTaskDate,
  changeTaskFollowed,
  CHANGE_OPEN_TASK,
  deleteTask,
  RESET_TASK,
  CHANGE_TASK,
} from "../../../services/actions/tasksAPI";
import Cookies from "js-cookie";
import { nowDate } from "../../../utils/api-utils";
import { TTask } from "../../../utils/types";
import { ClosedTask } from "./closed-task";

type TaskProps = {
  task: TTask;
  direction?: string;
};

export const Task = ({ task, direction }: TaskProps) => {
  const myId = Cookies.get("id");
  const dispatch = useAppDispatch();
  const [edited, setEdited] = useState(false);
  const editedId = useAppSelector((store) => store.editedTask.id);
  useEffect(() => {
    if (editedId !== task.id) setEdited(false);
  }, [editedId]);
  const open = () => {
    //console.log("opening");
    if (edited) return;
    setEdited(true);
    dispatch({ type: CHANGE_OPEN_TASK, task: task });
  };
  const save = (task: TTask) => {
    if (!edited) return;
    dispatch({ type: CHANGE_TASK, task: task, id: task.id });
    dispatch({ type: RESET_TASK });
    setEdited(false);
  };
  const deleteThisTask = (task: TTask) => {
    dispatch(deleteTask(task.id));
    dispatch({ type: RESET_TASK });
    setEdited(false);
  };
  const deleteRepeatedTasks = (task: TTask) => {
    dispatch(deleteTask(task.id, task.groupId));
    dispatch({ type: RESET_TASK });
    setEdited(false);
  };

  const isFollowed = () => {
    if (task.subscribers.some((s) => s.id == myId)) return true;
    return false;
  };
  const changeCheck = (markComplete: boolean) => {
    if (markComplete) dispatch(changeCompleteTaskDate(task.id, nowDate()));
    else dispatch(changeCompleteTaskDate(task.id, null));
  };
  const changeFollowed = (markFollowed: boolean) => {
    dispatch(changeTaskFollowed(task.id, markFollowed));
  };

  return (
    <div onClick={open}>
      {edited ? (
        <OpenTask
          save={save}
          deleteTask={deleteThisTask}
          deleteTasks={deleteRepeatedTasks}
          direction={direction}
          changeCheck={changeCheck}
        />
      ) : (
        <ClosedTask
          label={task.description}
          checked={!!task.completedAt}
          date={task.startAt}
          assignee={task.assignee}
          changeCheck={changeCheck}
          changeFollowed={changeFollowed}
          followed={direction || isFollowed() ? isFollowed() : undefined}
          direction={direction ? undefined : task.direction?.name}
          comments={task?.comments?.length}
        />
      )}
    </div>
  );
};

