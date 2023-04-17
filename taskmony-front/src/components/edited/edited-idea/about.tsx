import { changeIdeaDetails, CHANGE_IDEA_DETAILS } from "../../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import { AddBtn } from "../btn";
import add from "../../../images/add-light.svg";

export const About = () => {
    const dispatch = useAppDispatch();
    const description = useAppSelector((store) => store.editedIdea.details);
    const ideaId = useAppSelector((store) => store.editedIdea.id);
    const details = (
      <div className="flex gap-2 mr-8">
        <textarea
          placeholder={undefined}
          value={description}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === "Escape") e.target.blur();
          }}
          onChange={(e) =>
            dispatch({ type: CHANGE_IDEA_DETAILS, payload: e.target.value })
          }
          onBlur={(e) => {
            if (ideaId)
              dispatch(
                changeIdeaDetails(
                  ideaId,
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