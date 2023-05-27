import { useEffect, useState } from "react";
import { OpenIdea } from "./open-idea/open-idea";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import {
  changeIdeaFollowed,
  CHANGE_OPEN_IDEA,
  CHANGE_IDEAS,
  deleteIdea,
  RESET_IDEA,
  reviewIdea,
} from "../../../services/actions/ideasAPI";
import Cookies from "js-cookie";
import { TIdea } from "../../../utils/types";
import { ClosedIdea } from "./closed-idea";

type IdeaProps = {
  idea: TIdea;
  direction?: string;
  last: boolean;
};

export const Idea = ({ idea, direction, last }: IdeaProps) => {
  const myId = Cookies.get("id");
  const dispatch = useAppDispatch();
  const [edited, setEdited] = useState(false);
  const editedId = useAppSelector((store) => store.editedIdea.id);
  useEffect(() => {
    if (editedId !== idea.id) setEdited(false);
  }, [editedId]);
  const open = () => {
    //console.log("opening");
    if (edited) return;
    setEdited(true);
    dispatch({ type: CHANGE_OPEN_IDEA, idea: idea });
  };
  const save = (idea: TIdea) => {
    if (!edited) return;
    dispatch({ type: CHANGE_IDEAS, idea: idea });
    dispatch({ type: RESET_IDEA });
    setEdited(false);
  };
  const deleteThisIdea = (idea: TIdea) => {
    dispatch(deleteIdea(idea.id));
    dispatch({ type: RESET_IDEA });
    setEdited(false);
  };

  const isFollowed = () => {
    if (idea.subscribers.some((s) => s.id == myId)) return true;
    return false;
  };
  const changeFollowed = (markFollowed: boolean) => {
    dispatch(changeIdeaFollowed(idea.id, markFollowed));
  };
  const review = () => {
    dispatch(reviewIdea(idea.id));
  };

  return (
    <div onClick={open}>
      {edited ? (
        <OpenIdea
          save={save}
          deleteIdea={deleteThisIdea}
          direction={direction}
        />
      ) : (
        <ClosedIdea
          label={idea.description}
          last={last}
          changeFollowed={changeFollowed}
          review={review}
          followed={direction || isFollowed() ? isFollowed() : undefined}
          direction={direction ? null : idea.direction?.name}
          generation={idea.generation}
          comments={idea?.comments?.length}
        />
      )}
    </div>
  );
};
