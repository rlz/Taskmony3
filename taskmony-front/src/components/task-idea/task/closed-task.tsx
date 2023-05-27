import yes from "../../../images/checkbox-yes.svg";
import no from "../../../images/checkbox-no.svg";
import followBlue from "../../../images/followed.svg";
import followGray from "../../../images/follow.svg";
import divider from "../../../images/divider.svg";
import commentsI from "../../../images/comment2.svg";
import createdByI from "../../../images/by.svg";
import recurrentI from "../../../images/arrows-rotate.svg";

type TaskClosedProps = {
  label: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: string;
  createdBy?: string;
  direction?: string;
  changeCheck: Function;
  changeFollowed: Function;
  assignee: {
    displayName: string;
    id: string;
  };
  date: string;
};

export const ClosedTask = ({
  label,
  checked,
  followed,
  comments,
  recurrent,
  createdBy,
  direction,
  changeCheck,
  changeFollowed,
  assignee,
  date,
}: TaskClosedProps) => {
  return (
    <div className="uneditedTask w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <img
            src={checked ? yes : no}
            onClick={(e) => {
              e.stopPropagation();
              changeCheck(!checked);
            }}
            alt=""
          ></img>
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
        {typeof followed !== "undefined" && (
          <img
            className="w-4"
            src={followed ? followBlue : followGray}
            onClick={(e) => {
              e.stopPropagation();
              changeFollowed(!followed);
            }}
            alt=""
          ></img>
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
        {date.slice(0, 10) > new Date().toISOString().slice(0, 10) && (
          <TaskDetails label={date.slice(0, 10)} hasBorder />
        )}
        {assignee && (
          <TaskDetails
            label={`assignee: ${assignee ? assignee.displayName : "none"}`}
            hasBorder
            textColor={assignee ? undefined : "text-red-500"}
          />
        )}
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
    <div
      className={`flex flex-nowrap gap-1 ml-1  ${
        icon !== undefined ? "mr-5" : "mr-1"
      }`}
    >
      {icon && <img src={icon} alt=""></img>}
      <span
        className={`font-semibold inline whitespace-nowrap text-xs mr-1 ${
          textColor ? textColor : "text-blue-500"
        }`}
      >
        {label}
      </span>
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
