import { useState } from "react";
import {
  changeTaskDescription,
  CHANGE_TASK_DESCRIPTION,
} from "../../../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";
import { ChangeRepeatedValueModal } from "./repeated-modal";

export const Description = ({ description, setDescription, closeBtnRef }) => {
  const dispatch = useAppDispatch();
  const [showModal, setShowModal] = useState(null);
  const task = useAppSelector((store) => store.editedTask);
  return (
    <>
      {showModal && (
        <ChangeRepeatedValueModal
          changeThis={() => {
            setShowModal(null);
            dispatch(changeTaskDescription(task.id, description));
            closeBtnRef.click();
          }}
          changeAll={() => {
            setShowModal(null);
            dispatch(changeTaskDescription(task.id, description, task.groupId));
            closeBtnRef.click();
          }}
        />
      )}
      <input
        className={
          "w-full bg-slate-500 bg-opacity-0 font-semibold text-sm focus:outline-none placeholder:font-thin placeholder:text-black decoration-slate-50"
        }
        id="description"
        autoFocus
        placeholder={"describe a task"}
        autoComplete="off"
        value={description}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === "Escape") e.target.blur();
        }}
        onChange={(e) => setDescription(e.target.value)}
        onBlur={(e) => {
          if (description == task.description) return;
          dispatch({
            type: CHANGE_TASK_DESCRIPTION,
            payload: e.target.value,
          });
          if (task.groupId) {
            setShowModal(true);
          } else if (task.id)
            dispatch(changeTaskDescription(task.id, e.target.value));
        }}
      />
    </>
  );
};
