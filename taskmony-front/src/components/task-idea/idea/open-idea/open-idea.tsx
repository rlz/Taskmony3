import arrowUp from "../../../../images/arrow-up.svg";
import { useEffect, useRef } from "react";
import closeI from "../../../../images/delete2.svg";
import deleteI from "../../../../images/delete3.svg";
import shareI from "../../../../images/share.svg";
import { BigBtn } from "../../open-items-components/big-btn";
import arrowUpGray from "../../../../images/arrow-up-gray.svg";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";
import {
  RESET_IDEA
} from "../../../../services/actions/ideasAPI";
import { useLocation, useNavigate } from "react-router-dom";
import { Comments } from "../../open-items-components/comments/comments";
import { Details } from "./idea-details";
import { sendIdeaComment } from "../../../../services/actions/comments";
import { Description } from "./description";
import { About } from "./about";

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

export const OpenIdea = ({
  direction,
  save,
  close,
  deleteIdea,
  followed,
  recurrent,
}: IdeaProps) => {
  const idea = useAppSelector((store) => store.editedIdea);
  const { pathname } = useLocation();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const closeBtn = useRef(null);
  const saveBtn = useRef(null);
  const closeModal = () => {
    if (idea.description) {
      console.log("closing...");
      save(idea.details === "" ? { ...idea, details: null } : idea);
    }
  };
  const closeModalRef = useRef(closeModal);
  closeModalRef.current = closeModal;
  const onKeyPress = (event: any) => {
    if (event.key === "Escape") {
      console.log("Escape");
      if (idea.id && saveBtn.current) saveBtn.current.click();
      else if (!idea.id && closeBtn) closeBtn.current.click();
    }
    if (event.key === "Enter") {
      console.log("Enter");
      if (idea.id && saveBtn.current) saveBtn.current.click();
      else if (!idea.id) closeModalRef.current();
    }
  };
  useEffect(() => {
    document.addEventListener("keydown", onKeyPress);
    return () => {
      document.removeEventListener("keydown", onKeyPress);
    };
  }, []);
  return (
    <div className="editedIdea w-full bg-white rounded-lg drop-shadow-sm  pb-1">
      <div className={"gap-4 flex justify-between p-2 mb"}>
      <div className="flex  gap-2">
        <Description/>
        </div>
        {idea.id ? (
          <div className="relative z-30 flex gap-2">
            {/* <img
            alt=""
              src={shareI}
              onClick={() => navigate(`/idea/${idea.id}`,{ state: { from: pathname } })}
              className={"shareBtn w-4 mt-1 mr-1 cursor-pointer"}
            /> */}
            <img
              alt="delete button"
              src={deleteI}
              onClick={() => {
                if (deleteIdea) deleteIdea(idea);
              }}
              className={"deleteBtn w-4 mt-1 mr-1 cursor-pointer"}
            />
          </div>
        ) : (
          <img
            src={closeI}
            alt="close button"
            ref={closeBtn}
            onClick={() => {
              if (close) close();
              dispatch({ type: RESET_IDEA });
            }}
            className={"w-4 p-0.5 cursor-pointer"}
          />
        )}
      </div>
      <About />
      <Details fromDirection={direction} />
      {idea.id && (
        <Comments
          send={(input : string) => {
            dispatch(sendIdeaComment(idea.id, input));
          }}
          comments={idea.comments}
        />
      )}
      <div className={"w-full flex justify-end"}>
        {idea.description ? (
          !idea.id ? (
            <BigBtn label={"add an idea"} onClick={closeModal} />
          ) : (
            <img
            alt=""
              src={arrowUp}
              ref={saveBtn}
              onClick={closeModal}
              className={"closeBtn w-4 cursor-pointer mr-3 m-2"}
            ></img>
          )
        ) : !idea.id ? (
          <BigBtn label={"add an idea"} onClick={() => {}} unactive={true} />
        ) : (
          <img
          alt=""
            src={arrowUpGray}
            onClick={() => {}}
            className={"closeBtn w-4 mr-3 m-2"}
          ></img>
        )}
      </div>
    </div>
  );
};
