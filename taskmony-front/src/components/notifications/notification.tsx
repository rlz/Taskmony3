import arrowDown from "../../images/arrow-down.svg";
import follow from "../../images/followed.svg";
import divider from "../../images/divider.svg";
import createdByI from "../../images/by.svg";
import timeI from "../../images/time.svg";

import commentAdded from "../../images/comment1.svg";
import taskAssigned from "../../images/star.svg";

import itemAdded from "../../images/item-added.svg";
import itemRemoved from "../../images/item-removed.svg";
import itemEdited from "../../images/item-edited.svg";

import userAdded from "../../images/user-added.svg";
import userRemoved from "../../images/user-removed.svg";

type NotificationProps = {
  type: string;
  label: string;
  time?: string;
  createdBy?: string;
  direction?: string;
  details?: string;
  notRead?: boolean;
};

export const NotificationItem = ({
  type,
  label,
  time,
  createdBy,
  direction,
  details,
  notRead,
}: NotificationProps) => {
  const iconByType = (type: string) => {
    switch (type) {
      case "itemAdded":
        return itemAdded;
      case "itemDeleted":
        return itemRemoved;
      case "userAdded":
        return userAdded;
      case "userDeleted":
        return userRemoved;
      case "itemEdited":
        return itemEdited;
      case "taskAssigned":
        return taskAssigned;
      case "commentAdded":
        return commentAdded;
    }
  };

  return (
    <div className={`w-full drop-shadow-sm bg-white rounded-lg`}>
      <div className={"gap-4 flex justify-between p-2 mt-2 mb"}>
        <div>
          <div className="flex  gap-2">
            <img src={iconByType(type)}></img>
            <span className={"font-semibold text-sm"}>{label}</span>
          </div>
          <p className={"text-sm italic"}>{details}</p>
        </div>
      </div>
      <div className={"gap flex justify-start pb-2 w-full ml-1"}>
        {<Details label={direction} hasBorder />}
        {createdBy && (
          <Details icon={createdByI} label={`by ${createdBy}`} hasBorder />
        )}
        {time && <Details icon={timeI} label={`${time}`} />}
      </div>
    </div>
  );
};

type DetailsProps = {
  icon?: string;
  label?: string;
  hasBorder?: boolean;
  textColor?: string;
};

export const Details = ({
  icon,
  label,
  hasBorder,
  textColor,
}: DetailsProps) => {
  return (
    <div className={`flex flex-nowrap gap-1 mr-1  ml-1`}>
      {icon && <img src={icon}></img>}
      <span
        className={`font-semibold inline whitespace-nowrap text-xs mr-1 ${
          textColor ? textColor : "text-blue-500"
        }`}
      >
        {label}
      </span>
      {hasBorder && <img src={divider}></img>}
    </div>
  );
};

type BtnProps = {
  label: string;
  onClick: Function;
};

export const MoveBtn = ({ label, onClick }: BtnProps) => {
  return (
    <div
      className={"gap-4 flex justify-center bg-blue-500 rounded"}
      onClick={() => onClick()}
    >
      <img src={arrowDown}></img>
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
