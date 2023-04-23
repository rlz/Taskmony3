import yes from "../../../images/checkbox-yes.svg";
import no from "../../../images/checkbox-no.svg";
import arrowUp from "../../../images/arrow-up.svg";
import arrowUpGray from "../../../images/arrow-up-gray.svg";
import { useEffect, useRef, useState } from "react";
import closeI from "../../../images/delete2.svg";
import deleteI from "../../../images/delete3.svg";
import shareI from "../../../images/share.svg";
import { BigBtn } from "../big-btn";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import {
  changeTaskDescription,
  CHANGE_TASK_DESCRIPTION,
  RESET_TASK,
} from "../../../services/actions/tasksAPI";
import { sendTaskComment } from "../../../services/actions/comments";
import { useLocation, useNavigate } from "react-router-dom";
import { Comments } from "../comments/comments";
import { Description } from "./description";
import { Details } from "./task-details";
import { About } from "./about";
import {
  ChangeRepeatedModeModal,
  ChangeRepeatedValueModal,
  DeleteRepeatedValueModal,
  RepeatedModal,
} from "./repeated-modal";

type TaskProps = {
  label?: string;
  checked?: boolean;
  followed?: boolean;
  comments?: number;
  recurrent?: boolean;
  createdBy?: string;
  direction?: string;
  save: Function;
  close?: Function;
  changeCheck: Function;
  deleteTask?: Function;
  deleteTasks?: Function;
};

export const EditedTask = ({
  direction,
  save,
  close,
  deleteTask,
  deleteTasks,
  followed,
  recurrent,
  changeCheck,
}: TaskProps) => {
  const task = useAppSelector((store) => store.editedTask);
  const [showModal, setShowModal] = useState(null);
  const [description, setDescription] = useState<string>(task.description);
  const { pathname } = useLocation();
  const dispatch = useAppDispatch();
  const closeBtn = useRef(null);
  const saveBtn = useRef(null);
  const navigate = useNavigate();
  const closeModal = () => {
    if (task.description) {
      console.log("closing...");
      console.log(task);
      save(task.details === "" ? { ...task, details: null } : task);

    }
  };
  const closeModalRef = useRef(closeModal);
  closeModalRef.current = closeModal;
  const onKeyPress = (event: any) => {
    if (event.key === "Escape") {
      console.log("Escape");
      if (task.id && saveBtn.current) {
        console.log("clicking");
        saveBtn.current.click();
      } else if (!task.id && closeBtn.current) closeBtn.current.click();
    }
    if (event.key === "Enter") {
      if (task.groupId && event.target.id == "description" || event.target.id == "details") return; 
      console.log("Enter");
      if (task.id && saveBtn) {
        console.log("clicking");
        saveBtn.current.click();
      } else if (!task.id) {
        console.log(task);
        closeModalRef.current();
      }
    }
  };

  useEffect(() => {
    document.addEventListener("keydown", onKeyPress);
    return () => {
      document.removeEventListener("keydown", onKeyPress);
    };
  }, []);
  return (
    <div className="editedTask w-full bg-white rounded-lg drop-shadow-sm  pb-1">
      {showModal === "DELETE_MODAL" && (
        <DeleteRepeatedValueModal
          deleteThis={() => {
            setShowModal(null);
            if (deleteTask) deleteTask(task);
          }}
          deleteAll={() => {
            setShowModal(null);
            if (deleteTasks) deleteTasks(task);
          }}
        />
      )}
      {showModal === "REPEAT_MODE_MODAL" && <ChangeRepeatedModeModal />}
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          {task.id && (
            <img
              src={task.completedAt ? yes : no}
              onClick={(e) => {
                e.stopPropagation();
                changeCheck(!task.completedAt);
              }}
            ></img>
          )}
          <Description description={description} setDescription={setDescription} closeBtnRef={saveBtn.current}/>
        </div>
        {task.id ? (
          <div className="relative z-30 flex gap-2">
            <img
              src={shareI}
              onClick={() => {
                navigate(`/task/${task.id}`, { state: { from: pathname } });
              }}
              className={"shareBtn w-4 mt-1 mr-1 cursor-pointer"}
            />
            <img
              src={deleteI}
              onClick={() => {
                if (task.groupId) setShowModal("DELETE_MODAL");
                else {
                  if (deleteTask) deleteTask(task);
                }
              }}
              className={"deleteBtn w-4 mt-1 mr-1 cursor-pointer"}
            />
          </div>
        ) : (
          <img
            src={closeI}
            ref={closeBtn}
            onClick={() => {
              if (close) close();
              dispatch({ type: RESET_TASK });
            }}
            className={"w-4 p-0.5 cursor-pointer"}
          />
        )}
      </div>
      <About />
      <Details fromDirection={direction} />
      {task.id && (
        <Comments
          send={(input) => {
            dispatch(sendTaskComment(task.id, input));
          }}
          comments={task.comments}
        />
      )}
      <div className={"w-full flex justify-end"}>
        {description ? (
          !task.id ? (
            <BigBtn label={"add a task"} onClick={closeModal} color="blue" />
          ) : (
            <img
              src={arrowUp}
              onClick={closeModal}
              ref={saveBtn}
              className={"closeBtn w-4 cursor-pointer mr-3 m-2"}
            ></img>
          )
        ) : !task.id ? (
          <BigBtn label={"add a task"} onClick={() => {}} unactive={true} />
        ) : (
          <img
            src={arrowUpGray}
            onClick={() => {}}
            className={"closeBtn w-4 mr-3 m-2"}
          ></img>
        )}
      </div>
    </div>
  );
};
