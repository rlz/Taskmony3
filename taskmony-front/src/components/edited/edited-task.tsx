import yes from "../../images/checkbox-yes.svg";
import no from "../../images/checkbox-no.svg";
import divider from "../../images/divider.svg";
import commentsI from "../../images/comment2.svg";
import createdByI from "../../images/by.svg";
import arrowUp from "../../images/arrow-up.svg";
import arrowUpGray from "../../images/arrow-up-gray.svg";
// import recurrentI from "../images/recurrent.svg";
import recurrentI from "../../images/arrows-rotate.svg";
import { AddBtn } from "./btn";
import { Comment, CommentInput } from "./comment";
import { ItemPicker } from "./item-picker";
import { useEffect, useRef, useState } from "react";
import closeI from "../../images/delete2.svg";
import deleteI from "../../images/delete3.svg";
import add from "../../images/add-light.svg";
import { BigBtn } from "./big-btn";
import { DatePicker } from "./date-picker";
import { NumberPicker } from "./number-picker";
import followBlue from "../../images/followed.svg";
import followGray from "../../images/follow.svg";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import {
  changeTaskAssignee,
  changeTaskDescription,
  changeTaskDetails,
  changeTaskDirection,
  changeTaskRepeatMode,
  changeTaskRepeatUntil,
  changeTaskStartAt,
  CHANGE_TASK_ASSIGNEE,
  CHANGE_TASK_DESCRIPTION,
  CHANGE_TASK_DETAILS,
  CHANGE_TASK_DIRECTION,
  CHANGE_TASK_REPEAT_EVERY,
  CHANGE_TASK_REPEAT_MODE,
  CHANGE_TASK_REPEAT_UNTIL,
  CHANGE_TASK_REPEAT_WEEK_DAYS,
  CHANGE_TASK_START_DATE,
  RESET_TASK,
} from "../../services/actions/tasksAPI";
import { WeekPicker } from "./week-picker";
import { sendTaskComment } from "../../services/actions/comments";

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
  changeCheck: Function;
  deleteTask?: Function;
};

export const EditedTask = ({
  direction,
  save,
  close,
  deleteTask,
  followed,
  recurrent,
  changeCheck,
}: TaskProps) => {
  const task = useAppSelector((store) => store.editedTask);
  const dispatch = useAppDispatch();
  const closeBtn = useRef(null);
  const saveBtn = useRef(null);
  const closeModal = () => {
  if(task.description) {
    console.log("closing...")
    console.log(task);
    save(task.details===""?{...task,details:null}:task)}
  }
const closeModalRef = useRef(closeModal);
closeModalRef.current = closeModal;
  const onKeyPress = (event: any) => {
    if (event.key === "Escape") {
      console.log("Escape")
      if(task.id && saveBtn.current) {console.log("clicking");saveBtn.current.click()}
      else if (!task.id && closeBtn.current) closeBtn.current.click();
    }
    if (event.key === "Enter") {
      console.log("Enter")
      if(task.id && saveBtn) {console.log("clicking");saveBtn.current.click();}
      else if (!task.id && task.description) {console.log(task);closeModalRef.current()}
    }
    }

  useEffect(() => {
    document.addEventListener('keydown', onKeyPress)
    return () => {
      document.removeEventListener('keydown', onKeyPress)
    };
}, []);
  return (
    <div className="editedTask w-full bg-white rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          {task.id && <img
            src={task.completedAt ? yes : no}
            onClick={(e) => {
              e.stopPropagation();
              changeCheck(!task.completedAt);
            }}
          ></img>}
          <input
            className={`font-semibold text-sm focus:outline-none placeholder:font-thin placeholder:text-black decoration-slate-50`}
            id="description"
            autoFocus
            autoComplete="off"
            placeholder={"describe a task"}
            value={task.description}
            onChange={(e) =>
              dispatch({
                type: CHANGE_TASK_DESCRIPTION,
                payload: e.target.value,
              })
            }
            onKeyDown={(e)=>
              {if(e.key === "Enter" || e.key === "Escape") e.target.blur();}
            }
            onBlur={(e) => {
              if (task.id)
                dispatch(changeTaskDescription(task.id, e.target.value));
            }}
          />
        </div>
        {task.id ? (
          <img
            src={deleteI}
            onClick={() => {
              if(deleteTask) deleteTask(task);
            }}
            className={"deleteBtn w-4 mt-1 mr-1 cursor-pointer"}
          />
        ) : (
          <img
            src={closeI}
            ref={closeBtn}
            onClick={() => {
              if(close) close();
              dispatch({ type: RESET_TASK });
            }}
            className={"w-4 p-0.5 cursor-pointer"}
          />
        )}
      </div>
      <Description />
      <Details fromDirection={direction} />
      {task.id && <Comments />}
      <div className={"w-full flex justify-end"}>
        {task.description ? (!task.id ? (
          <BigBtn
            label={"add a task"}
            onClick={closeModal}
            color="blue"
          />
        ) : (
          <img
            src={arrowUp}
            onClick={closeModal}
            ref={saveBtn}
            className={"closeBtn w-4 cursor-pointer mr-3 m-2"}
          ></img>
        )) :
        (!task.id ? (
          <BigBtn
            label={"add a task"}
            onClick={() => {}}
            unactive={true}
          />
        ) : (
          <img
            src={arrowUpGray}
            onClick={() => {}}
            className={"closeBtn w-4 mr-3 m-2"}
          ></img>
        ))}
      </div>
    </div>
  );
};

const Description = () => {
  const dispatch = useAppDispatch();
  const description = useAppSelector((store) => store.editedTask.details);
  const taskId = useAppSelector((store) => store.editedTask.id);
  const details = (
    <div className="flex gap-2 mr-8">
      <textarea
        placeholder={undefined}
        value={description}
        onKeyDown={(e)=>
          {if(e.key === "Enter" || e.key === "Escape") e.target.blur();}
        }
        onChange={(e) =>
          dispatch({ type: CHANGE_TASK_DETAILS, payload: e.target.value })
        }
        onBlur={(e) => {
          if (taskId)
            dispatch(
              changeTaskDetails(
                taskId,
                e.target.value === "" ? null : e.target.value
              )
            );
        }}
        className="text-black font-light underline placeholder:text-black placeholder:font-light 
        placeholder:underline 
        focus:outline-none
        w-full resize-none"
        // rows={(description == "") ? 1 : undefined}
        rows={1}
      />
    </div>
  );

  return (
    <div className="font-semibold text-sm text-blue-500 ml-2 mb-2">
      {description === null ? (
        <AddBtn
          label={"add details"}
          icon={add}
          onClick={() =>
            dispatch({ type: CHANGE_TASK_DETAILS, payload: "details" })
          }
        />
      ) : (
        details
      )}
    </div>
  );
};
type DetailsProps = {fromDirection?: string | null}
const Details = ({ fromDirection } : DetailsProps) => {
  const dispatch = useAppDispatch();
  const task = useAppSelector((store) => store.editedTask);
  const repeatOptions = ["no", "daily", "custom"];
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt == null
  );

  const repeatModeTranslator = (mode : string) => {
    switch (mode) {
      case "no":
        return null;
      case "daily":
        return "DAY";
      case "custom":
        return "WEEK";
      case null:
        return "no";
      case "DAY":
        return "daily";
      case "WEEK":
        return "custom";
      default: return null;
    }
    return null;
  };
  const defaultUntilDate = () => {
    const date = new Date(Date.now());
    return new Date(date.setFullYear(date.getFullYear() + 1));
  };
  const members = directions.filter((d) => d.id == task.direction?.id)[0]
    ?.members;

  return (
    <div className={"gap flex justify-start pb-2 w-full ml-1"}>
      {!fromDirection && (
        <ItemPicker
          title={"direction"}
          option={task.direction?.name ? task.direction?.name : "none"}
          options={["none", ...directions.map((dir) => dir.name)]}
          onChange={(index : number) => {
            const payload = index == 0 ? null : directions[index - 1];
            dispatch({ type: CHANGE_TASK_DIRECTION, payload: payload });
            if (task.id && payload) dispatch(changeTaskDirection(task.id, payload));
          }}
          hasBorder
        />
      )}
      {members && (
        <ItemPicker
          title={"assignee"}
          options={members.map((m) => m.displayName)}
          option={task.assignee?.displayName ? task.assignee?.displayName : "undefined"}
          onChange={(index : number) => {
            const payload = members[index];
            dispatch({ type: CHANGE_TASK_ASSIGNEE, payload: payload });
            if (task.id) dispatch(changeTaskAssignee(task.id, payload));
          }}
          hasBorder
          width="w-24"
        />
      )}
      <DatePicker
        title={"start date"}
        date={task.startAt ? new Date(task.startAt) : new Date()}
        hasBorder
        onChange={(value  : string) => {
          dispatch({ type: CHANGE_TASK_START_DATE, payload: value });
          if (task.id) dispatch(changeTaskStartAt(task.id, value));
        }}
      />
      <ItemPicker
        title={"repeated"}
        options={repeatOptions}
        option={repeatModeTranslator(task.repeatMode) || "no"}
        onChange={(index : number) => {
          dispatch({
            type: CHANGE_TASK_REPEAT_MODE,
            payload: repeatModeTranslator(repeatOptions[index]),
          });
          //when mode is weekly or daily
          if (
            repeatOptions[index] == "daily" ||
            repeatOptions[index] == "custom"
          ) {
            dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: 1 });
            dispatch({
              type: CHANGE_TASK_REPEAT_UNTIL,
              payload: new Date(defaultUntilDate())
                .toISOString()
                .substring(0, 10),
            });
          }
          //when mode is weekly
          if (repeatOptions[index] == "custom")
            dispatch({
              type: CHANGE_TASK_REPEAT_WEEK_DAYS,
              payload: ["MONDAY"],
            });
          if (task.id)
            dispatch(
              changeTaskRepeatMode(
                task.id,
                repeatModeTranslator(repeatOptions[index]),
                task.repeatEvery,
                task.weekDays
              )
            );
        }}
        hasBorder
      />
      {task.repeatMode && (
        <DatePicker
          title={"until"}
          min={new Date(task.startAt)}
          date={
            task.repeatUntil ? new Date(task.repeatUntil) : defaultUntilDate()
          }
          hasBorder
          onChange={(value : string) => {
            dispatch({ type: CHANGE_TASK_REPEAT_UNTIL, payload: value });
            if (task.id) dispatch(changeTaskRepeatUntil(task.id, value));
          }}
        />
      )}
      {task.repeatMode === "WEEK" && (
        <>
          <NumberPicker
            title={"every"}
            min={1}
            max={9}
            after={"week(s)"}
            value={task.repeatEvery}
            onChange={(value  : string) => {
              dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: value });
              if (task.id)
                dispatch(
                  changeTaskRepeatMode(
                    task.id,
                    task.repeatMode,
                    value,
                    task.weekDays
                  )
                );
            }}
            hasBorder
          />
          <WeekPicker
            value={task.weekDays}
            onChange={(value : Array<string>) => {
              dispatch({ type: CHANGE_TASK_REPEAT_WEEK_DAYS, payload: value });
              if (task.id)
                dispatch(
                  changeTaskRepeatMode(
                    task.id,
                    task.repeatMode,
                    task.repeatEvery,
                    value
                  )
                );
            }}
          />
        </>
      )}
    </div>
  );
};

const Comments = () => {
  const taskId = useAppSelector((store) => store.editedTask.id);
  const dispatch = useAppDispatch();
  const comments = useAppSelector((store) => store.editedTask.comments);
  const [showInput, setShowInput] = useState<boolean>(false);
  const [commentInput, setCommentInput] = useState<string>("");
  const sendComment = () => {
    dispatch(sendTaskComment(taskId, commentInput));
    setShowInput(false);
    setCommentInput("");
  };

  return (
    <>
      {comments?.map((comment,index) => (
        <Comment
          key={index}
          text={comment.text}
          author={comment.createdBy.displayName}
          time={comment.createdAt}
        />
      ))}
      {showInput && (
        <CommentInput
          commentValue={commentInput}
          changeComment={setCommentInput}
          send={sendComment}
        />
      )}
      <div className="flex justify-center p-1">
        <AddBtn
          label={commentInput ? "send comment" : "add a new comment"}
          icon={commentInput ? undefined : add}
          onClick={showInput ? sendComment : () => setShowInput(true)}
        />
      </div>
    </>
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
