import followBlue from "../images/followed.svg";
import followGray from "../images/follow.svg";
import divider from "../images/divider.svg";
import commentsI from "../images/comment2.svg";
import createdByI from "../images/by.svg";
import { useState } from "react";
import { EditedIdea } from "./edited/edited-idea";
import { useAppDispatch } from "../utils/hooks";
import {
  changeIdeaFollowed,
  CHANGE_OPEN_IDEA,
  CHANGE_IDEAS,
  deleteIdea,
  RESET_IDEA,
  reviewIdea,
} from "../services/actions/ideasAPI";
import { getCookie } from "../utils/cookies";
import postponeGray from "../images/circle-down-gray.svg";
import postponeBlue from "../images/circle-down-blue.svg";
import { TIdea } from "../utils/types";

type IdeaProps = {
  idea: TIdea, direction: string, last: boolean
}

export const Idea = ({ idea, direction, last } : IdeaProps) => {
  const myId = getCookie("id");
  const dispatch = useAppDispatch();
  const [edited, setEdited] = useState(false);
  const open = () => {
    console.log("opening");
    if (edited) return;
    setEdited(true);
    dispatch({ type: CHANGE_OPEN_IDEA, idea: idea });
  };
  const save = (idea : TIdea) => {
    if (!edited) return;
    dispatch({ type: CHANGE_IDEAS, idea: idea });
    dispatch({ type: RESET_IDEA });
    setEdited(false);
  };
  const deleteThisIdea = (idea : TIdea) => {
    console.log(idea);
    dispatch(deleteIdea(idea.id));
    dispatch({ type: RESET_IDEA });
    setEdited(false);
  };

  const isFollowed = () => {
    if (idea.subscribers.some((s) => s.id == myId)) return true;
    return false;
  };
  const changeFollowed = (markFollowed : boolean) => {
    console.log("following", markFollowed);
    dispatch(changeIdeaFollowed(idea.id, markFollowed));
  };
  const review = () => {
    dispatch(reviewIdea(idea.id));
  };

  return (
    <div onClick={open}>
      {edited ? (
        <EditedIdea
          save={save}
          deleteIdea={deleteThisIdea}
          direction={direction}
        />
      ) : (
        <IdeaUnedited
          label={idea.description}
          last={last}
          changeFollowed={changeFollowed}
          review={review}
          reviewedAt={idea.reviewedAt}
          followed={direction || isFollowed() ? isFollowed() : undefined}
          direction={direction ? null : idea.direction?.name}
          generation={idea.generation}
          comments={idea?.comments?.length}
        />
      )}
    </div>
  );
};

type IdeaUneditedProps = {
  label: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: string;
  createdBy?: string;
  direction?: string | null;
  last: boolean;
  generation: string;
  changeFollowed: Function;
  review: Function;
  reviewedAt: string
};

export const IdeaUnedited = ({
  label,
  checked,
  followed,
  comments,
  createdBy,
  generation,
  reviewedAt,
  direction,
  changeFollowed,
  review,
  last,
}: IdeaUneditedProps) => {
  return (
    <div className="uneditedIdea w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
        <div className="relative z-30 flex gap-2">
          {typeof followed !== "undefined" && (
            <img
              className="w-4"
              src={followed ? followBlue : followGray}
              onClick={(e) => {
                e.stopPropagation();
                changeFollowed(!followed);
              }}
            ></img>
          )}

          <img
            src={last ? postponeGray : postponeBlue}
            className="w-4"
            onClick={(e) => {
              e.stopPropagation();
              if (last) return;
              review();
            }}
          ></img>
        </div>
      </div>
      <div className={"gap flex justify-start pb-2 w-full ml-1"}>
        {/* <IdeaDetails icon={createdByI} label={`${reviewedAt}`} hasBorder /> */}
        {createdBy && (
          <IdeaDetails icon={createdByI} label={`by ${createdBy}`} hasBorder />
        )}
        {
          <IdeaDetails
            icon={commentsI}
            label={comments ? comments.toString() : "0"}
            hasBorder
          />
        }
        {direction && (
          <IdeaDetails
            label={direction}
            textColor="text-yellow-500"
            hasBorder
          />
        )}
        {
          <IdeaDetails
            label={"  " + generation.toLowerCase().replaceAll("_", " ")}
            textColor="text-yellow-500"
          />
        }
      </div>
    </div>
  );
};

type IdeaDetailsProps = {
  icon?: string;
  label?: string;
  hasBorder?: boolean;
  textColor?: string;
};

export const IdeaDetails = ({
  icon,
  label,
  hasBorder,
  textColor,
}: IdeaDetailsProps) => {
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
