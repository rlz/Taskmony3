import backI from "../images/arrow-left.svg";
import arrowUp from "../images/arrow-up.svg";
import { useEffect, useRef } from "react";
import arrowUpGray from "../images/arrow-up-gray.svg";
import useIsFirstRender, { useAppDispatch, useAppSelector } from "../utils/hooks";
import { useLocation, useNavigate } from "react-router-dom";
import { Description } from "../components/edited/edited-idea/description";
import { About } from "../components/edited/edited-idea/about";
import { Details } from "../components/edited/edited-idea/idea-details";
import { Comments } from "../components/edited/comments/comments";
import { sendIdeaComment } from "../services/actions/comments";
import { CHANGE_OPEN_IDEA } from "../services/actions/ideasAPI";

export const IdeaPage = () => {
    const location = useLocation();
    const from = location.state?.from;
    const navigate = useNavigate();
 
  return (
    <>
      <img src={backI} className={`w-4 m-5 cursor-pointer ${!from?'invisible':''}`} onClick={() => {if(from) navigate(from)}}/>
      <IdeaInfo save={undefined} />
    </>
  );
};

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

const IdeaInfo = ({
  direction,
  save,
  close,
  deleteIdea,
  followed,
  recurrent,
}: IdeaProps) => {
    const dispatch = useAppDispatch();
    const isFirst = useIsFirstRender();
    const ideaId = location.pathname.split("/")[2];
    const ideas = useAppSelector((store) => store.ideas.items);
    const myIdeas = ideas.filter(i=> i.id == ideaId)
    if(myIdeas[0]) dispatch({type: CHANGE_OPEN_IDEA, idea: myIdeas[0]});
  const idea = useAppSelector((store) => store.editedIdea);
  const navigate = useNavigate();
  const closeBtn = useRef(null);
  const saveBtn = useRef(null);
  const onKeyPress = (event: any) => {
    if (event.key === "Escape") {
      console.log("Escape");
      if (idea.id && saveBtn.current) saveBtn.current.click();
    }
    if (event.key === "Enter") {
      console.log("Enter");
      if (idea.id && saveBtn.current) saveBtn.current.click();

    }
  };
  useEffect(() => {
    document.addEventListener("keydown", onKeyPress);
    return () => {
      document.removeEventListener("keydown", onKeyPress);
    };
  }, []);
  if(isFirst && idea.id == "")
  return(
    <div className="flex items-center justify-center mt-8">
    </div>
  )
  if(myIdeas && myIdeas.length === 0 && !isFirst)
  return(
    <div className="flex items-center justify-center mt-8">
    <p className="font-semibold">You cannot access this idea or it does not exist</p>
    </div>
  )
  return (
    <div className="m-4 editedIdea rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <Description />
      </div>
      <About />
      <Details fromDirection={direction} />
      {idea.id && (
        <Comments
          send={(input) => {
            dispatch(sendIdeaComment(idea.id, input));
          }}
          comments={idea.comments}
        />
      )}
    </div>
  );
};
