import divider from "../../images/divider.svg";
import arrowUp from "../../images/arrow-up.svg";
// import recurrentI from "../images/recurrent.svg";
import { AddBtn } from "./btn";
import { Comment, CommentInput } from "./comment";
import { ItemPicker } from "./item-picker";
import { useState } from "react";
import closeI from "../../images/delete2.svg";
import deleteI from "../../images/delete3.svg";
import add from "../../images/add-light.svg";
import { BigBtn } from "./big-btn";
import arrowUpGray from "../../images/arrow-up-gray.svg";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import {
  changeIdeaDescription,
  changeIdeaDetails,
  changeIdeaDirection,
  changeIdeaGeneration,
  CHANGE_IDEA_DESCRIPTION,
  CHANGE_IDEA_DETAILS,
  CHANGE_IDEA_DIRECTION,
  CHANGE_IDEA_GENERATION,
  RESET_IDEA,
} from "../../services/actions/ideasAPI";
import { sendIdeaComment } from "../../services/actions/comments";

type IdeaProps = {
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
  deleteIdea?: Function;
};

export const EditedIdea = ({
  direction,
  save,
  close,
  deleteIdea,
  followed,
  recurrent,
}: IdeaProps) => {
  const idea = useAppSelector((store) => store.editedIdea);
  const dispatch = useAppDispatch();
  return (
    <div className="editedIdea w-full bg-white rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <input
            className={"font-semibold text-sm focus:outline-none underline"}
            placeholder={undefined}
            value={idea.description}
            onChange={(e) =>
              dispatch({
                type: CHANGE_IDEA_DESCRIPTION,
                payload: e.target.value,
              })
            }
            onBlur={(e) => {
              if (idea.id)
                dispatch(changeIdeaDescription(idea.id, e.target.value));
            }}
          />
        </div>
        {idea.id ? (
          <img
          alt="delete button"
            src={deleteI}
            onClick={() => {
              if (deleteIdea) deleteIdea(idea);
            }}
            className={"deleteBtn w-4 mt-1 mr-1 cursor-pointer"}
          />
        ) : (
          <img
            src={closeI}
            alt="close button"
            onClick={() => {
              if (close) close();
              dispatch({ type: RESET_IDEA });
            }}
            className={"w-4 p-0.5 cursor-pointer"}
          />
        )}
      </div>
      <Description />
      <Details fromDirection={direction} />
      {idea.id && <Comments />}
      <div className={"w-full flex justify-end"}>
      {idea.description ? (!idea.id ? (
          <BigBtn
            label={"add an idea"}
            onClick={() => save(idea.details===""?{...idea,details:null}:idea)}
          />
        ) : (
          <img
            src={arrowUp}
            onClick={() => save(idea.details===""?{...idea,details:null}:idea)}
            className={"closeBtn w-4 cursor-pointer mr-3 m-2"}
          ></img>
        )) :
        (!idea.id ? (
          <BigBtn
            label={"add an idea"}
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
  const description = useAppSelector((store) => store.editedIdea.details);
  const ideaId = useAppSelector((store) => store.editedIdea.id);
  const details = (
    <div className="flex gap-2 mr-8">
      <textarea
        placeholder={undefined}
        value={description}
        onChange={(e) =>
          dispatch({ type: CHANGE_IDEA_DETAILS, payload: e.target.value })
        }
        onBlur={(e) => {
          if (ideaId) dispatch(changeIdeaDetails(ideaId, e.target.value === "" ? null : e.target.value));
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
            dispatch({ type: CHANGE_IDEA_DETAILS, payload: "details" })
          }
        />
      ) : (
        details
      )}
    </div>
  );
};

type DetailsProps = { fromDirection?: string | null };
const Details = ({ fromDirection }: DetailsProps) => {
  const dispatch = useAppDispatch();
  const idea = useAppSelector((store) => store.editedIdea);
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt == null
  );
  const categories = ["hot", "later", "too good to delete"];

  return (
    <div className={"gap flex justify-start pb-2 w-full ml-1"}>
      <ItemPicker
        title={"category"}
        option={
          idea.generation
            ? idea.generation.toLowerCase().replaceAll("_", " ")
            : "hot"
        }
        options={categories}
        onChange={(index: number) => {
          const payload = categories[index];
          dispatch({ type: CHANGE_IDEA_GENERATION, payload: payload });
          if (idea.id) dispatch(changeIdeaGeneration(idea.id, payload));
        }}
        hasBorder
      />
      {!fromDirection && (
        <ItemPicker
          title={"direction"}
          option={idea.direction?.name ? idea.direction?.name : "none"}
          options={["none", ...directions.map((dir) => dir.name)]}
          onChange={(index: number) => {
            const payload = index === 0 ? null : directions[index - 1];
            dispatch({ type: CHANGE_IDEA_DIRECTION, payload: payload });
            if (idea.id && payload)
              dispatch(changeIdeaDirection(idea.id, payload));
          }}
          hasBorder
        />
      )}
    </div>
  );
};

const Comments = () => {
  const ideaId = useAppSelector((store) => store.editedIdea.id);
  const dispatch = useAppDispatch();
  const comments = useAppSelector((store) => store.editedIdea.comments);
  const [showInput, setShowInput] = useState<boolean>(false);
  const [commentInput, setCommentInput] = useState<string>("");
  const sendComment = () => {
    dispatch(sendIdeaComment(ideaId, commentInput));
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
      {icon && <img src={icon} alt=""></img>}
      <span
        className={
          "font-semibold inline whitespace-nowrap text-xs text-blue-500 mr-1 " +
          textColor
        }
      >
        {label}
      </span>
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
