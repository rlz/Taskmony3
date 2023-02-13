import yes from "../../images/checkbox-yes.svg";
import no from "../../images/checkbox-no.svg";
import follow from "../../images/followed.svg";
import divider from "../../images/divider.svg";
import commentsI from "../../images/comment2.svg";
import createdByI from "../../images/by.svg";
// import recurrentI from "../images/recurrent.svg";
import recurrentI from "../../images/arrows-rotate.svg";
import { AddBtn } from "./btn";
import { Comment, CommentInput } from "./comment";
import { ItemPicker } from "./item-picker";
import { useState } from "react";
import deleteI from "../../images/delete.svg";
import add from "../../images/add-light.svg";
import { SaveBtn } from "./save-btn";
import { DatePicker } from "./date-picker";
import { NumberPicker } from "./number-picker";
import followBlue from "../../images/followed.svg";
import followGray from "../../images/follow.svg";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { CHANGE_TASK_DESCRIPTION } from "../../services/actions/tasksAPI";
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
};

export const EditedTask = ({
  label,
  checked,
  followed,
  comments,
  recurrent,
  createdBy,
  direction,
  save,
}: TaskProps) => {
  const task = useAppSelector(
    (store) => store.editedTask
  );
  const dispatch = useAppDispatch()
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <img
            src={typeof checked === "undefined" || !checked ? no : yes}
          ></img>
          <input
            className={"font-semibold text-sm focus:outline-none underline"}
            placeholder={"task name"}
            value={task.description}
            onChange={(e)=>dispatch({type:CHANGE_TASK_DESCRIPTION,payload:e.target.value})}
          />
        </div>
        {typeof followed !== "undefined" && (
          <img className="w-4" src={followed ? followBlue : followGray}></img>
        )}
      </div>
      <Description />
      <Details recurrent={recurrent} />
      <Comments comments={task.comments} taskId={task.id}/>
      <SaveBtn label={"save"} onClick={()=>save(task)} />
    </div>
  );
};

const Description = () => {
  const [hasDetails, setHasDetails] = useState(false);
  const [description, setDescription] = useState<string>("");
  const details = (
    <div className="flex gap-2 mr-8">
      <img
        src={deleteI}
        className="cursor-pointer"
        onClick={() => setHasDetails(false)}
      ></img>
      <textarea
        placeholder={"details"}
        value={description}
        onChange={(e) => setDescription(e.target.value)}
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
      {hasDetails ? (
        details
      ) : (
        <AddBtn
          label={"add details"}
          icon={add}
          onClick={() => setHasDetails(true)}
        />
      )}
    </div>
  );
};

const Details = ({ recurrent }) => {
  const [isReccurent, setIsRecurrent] = useState("no");
  const [direction, setDirection] = useState("none");
  const directions = useAppSelector((store) => store.directions.items).filter(i=>i.deletedAt == null);

  const defaultUntilDate = () => {
    const date = new Date(Date.now());
    return date.setFullYear(date.getFullYear() + 1)
  }

  const customPicker = (
    <NumberPicker title={"every"} min={2} max={10} after={"days"} hasBorder />
  );

  return (
    <div className={"gap flex justify-start pb-2 w-full ml-1"}>
       <ItemPicker
        title={"direction"}
        option={"none"}
        options={["none",...directions.map(dir=>dir.name)]}
        onChange={() => {}}
        hasBorder
      />
      <ItemPicker
        title={"assignee"}
        option={"none"}
        options={["none", "Ann Smith", "Alexander Ivanov"]}
        onChange={() => {}}
        hasBorder
        width="w-24"
      />
      <DatePicker title={"start date"} date={Date.now()} hasBorder />
      <ItemPicker
        title={"repeated"}
        options={["no", "daily","custom"]}
        option={recurrent}
        onChange={setIsRecurrent}
        hasBorder
      />
      {isReccurent !== "no" && 
          <DatePicker title={"until"} date={defaultUntilDate()} hasBorder />
      }
      {isReccurent === "custom" && 
        <>
                <NumberPicker
        title={"every"}
        min={1}
        max={9}
        after={"week(s)"}
        hasBorder
      />
      <WeekPicker/>
        </>

    }
    </div>
  )};

const Comments = () => {
  const taskId = useAppSelector(
    (store) => store.editedTask.id
  );
  const dispatch = useAppDispatch();
  const comments = useAppSelector(
    (store) => store.editedTask.comments
  );
  const [showInput, setShowInput] = useState<boolean>(false);
  const [commentInput, setCommentInput] = useState<string>("");
  const sendComment = () => {
    dispatch(sendTaskComment(taskId,commentInput));
    setShowInput(false);
    setCommentInput("");
  }

  return (
    <>
      {comments.map(comment =>
                <Comment text={comment.text} author={comment.createdBy.displayName} time={comment.createdAt} />
      )}
      {showInput && <CommentInput commentValue={commentInput} changeComment={setCommentInput}/>}
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
