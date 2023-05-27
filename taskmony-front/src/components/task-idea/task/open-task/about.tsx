import {
  CHANGE_TASK_DETAILS,
  changeTaskDetails,
} from "../../../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";
import { AddBtn } from "../../open-items-components/btn";
import add from "../../../../images/add-light.svg";
import { useState } from "react";
import { ChangeRepeatedValueModal } from "./repeated-modal";

export const About = () => {
  const dispatch = useAppDispatch();
  const [showModal, setShowModal] = useState(null);
  const initialDetails = useAppSelector((store) => store.editedTask.details);
  const [description, setDescription] = useState<string>(
    initialDetails == "null" ? null : initialDetails
  );

  const task = useAppSelector((store) => store.editedTask);
  const details = (
    <div className="flex gap-2 mr-8">
      <textarea
        placeholder={undefined}
        value={description}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === "Escape") e.target.blur();
        }}
        onChange={(e) => setDescription(e.target.value)}
        onBlur={(e) => {
          if (description == initialDetails) return;
          dispatch({
            type: CHANGE_TASK_DETAILS,
            payload: e.target.value,
          });
          if (task.groupId) {
            setShowModal(true);
          } else if (task.id)
            dispatch(
              changeTaskDetails(
                task.id,
                e.target.value === "" ? null : e.target.value
              )
            );
        }}
        className="bg-slate-500 bg-opacity-0 text-black font-light underline placeholder:text-black placeholder:font-light 
          placeholder:underline 
          focus:outline-none
          w-full resize-none"
        // rows={(description == "") ? 1 : undefined}
        rows={1}
        id="details"
      />
    </div>
  );

  return (
    <div className="font-semibold text-sm text-blue-500 ml-2 mb-2">
      {showModal && (
        <ChangeRepeatedValueModal
          changeThis={() => {
            setShowModal(null);
            dispatch(changeTaskDetails(task.id, description));
          }}
          changeAll={() => {
            setShowModal(null);
            dispatch(changeTaskDetails(task.id, description, task.groupId));
          }}
        />
      )}
      {description === null ? (
        <AddBtn
          label={"add details"}
          icon={add}
          onClick={() => {
            dispatch({ type: CHANGE_TASK_DETAILS, payload: "details" });
            setDescription("details");
          }}
        />
      ) : (
        details
      )}
    </div>
  );
};
