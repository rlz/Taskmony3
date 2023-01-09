import yes from "../images/checkbox-yes.svg";
import no from "../images/checkbox-yes.svg";
import follow from "../images/followed.svg";

import commentsI from "../images/comments.svg";
import createdByI from "../images/by.svg";
import recurrentI from "../images/recurrent.svg";

type TaskProps = {
  label: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: string;
  createdBy?: string;
};

export const Task = ({
  label,
  checked,
  followed,
  comments,
  recurrent,
  createdBy,
}: TaskProps) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <img src={checked ? yes : no}></img>
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
        {followed && <img src={follow}></img>}
      </div>
      <div
      className={
        "gap flex justify-start pl-2 pb-2 w-full"
      }
    >
      {recurrent && <TaskDetails
        icon={recurrentI} label={recurrent} hasBorder
      />}
       {createdBy && <TaskDetails
        icon={createdByI} label={`by ${createdBy}`} hasBorder
      />}
      {<TaskDetails
        icon={commentsI} label={comments ? comments.toString() : "0"}
      />}
    </div> 
    </div>
  );
};

type TaskDetailsProps = {
    icon: string; label?: string; hasBorder?: boolean
};

export const TaskDetails = ({
  icon, label, hasBorder
}: TaskDetailsProps) => {
  return (
        <div className="flex flex-nowrap gap-2 mr-1 ml-2">
          <img src={icon}></img>
          <span className={"font-semibold inline whitespace-nowrap text-xs text-blue-500"}>
            {label}
          </span>
          {hasBorder && <div className="border border-grey-60"/>}
        </div>
      )};
