import arrowDown from "../images/arrow-down.svg";
import follow from "../images/followed.svg";
import divider from "../images/divider.svg";
import commentsI from "../images/comment2.svg";
import createdByI from "../images/by.svg";
import followBlue from "../images/followed.svg";
import followGray from "../images/follow.svg";
import postponeBlue from "../images/circle-down-blue.svg";
import postponeGray from "../images/circle-down-gray.svg";
// import recurrentI from "../images/recurrent.svg";
import recurrentI from "../images/arrows-rotate.svg";
import { AddBtn } from "./add-btn/add-btn";
import { EditedIdea } from "./edited/edited-idea";
import { useState } from "react";

export const Idea = (props) => {
  const [edited, setEdited] = useState(false);
  const open = () => {
    if (edited) return;
    setEdited(true);
  };
  const save = () => {
    if (!edited) return;
    setEdited(false);
  };
  return (
    <div onClick={open}>
      {edited ? (
        <EditedIdea {...props} save={save} />
      ) : (
        <UneditedIdea {...props} />
      )}
    </div>
  );
};

type IdeaProps = {
  label: string;
  followed?: boolean;
  comments?: number;
  createdBy?: string;
  direction?: string;
  last?: boolean;
};

export const UneditedIdea = ({
  label,
  followed,
  comments,
  createdBy,
  direction,
  last,
}: IdeaProps) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
        <div className="relative z-30 flex gap-2">
          {typeof followed !== "undefined" && (
            <img className="w-4" src={followed ? followBlue : followGray}></img>
          )}
          <img src={last ? postponeGray : postponeBlue} className="w-4"></img>
        </div>
      </div>
      <div className={"gap flex justify-start pb-2 w-full ml-1"}>
        {createdBy && (
          <Details icon={createdByI} label={`by ${createdBy}`} hasBorder />
        )}
        {
          <Details
            icon={commentsI}
            label={comments ? comments.toString() : "0"}
            hasBorder
          />
        }
        {<Details label={direction} textColor="text-yellow-500" />}
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
