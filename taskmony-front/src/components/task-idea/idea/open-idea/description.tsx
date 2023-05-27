import {
  changeIdeaDescription,
  CHANGE_IDEA_DESCRIPTION,
} from "../../../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";

export const Description = () => {
  const dispatch = useAppDispatch();
  const idea = useAppSelector((store) => store.editedIdea);
  return (
    <input
      className={
        "bg-slate-500 bg-opacity-0 font-semibold text-sm focus:outline-none placeholder:font-thin placeholder:text-black decoration-slate-50"
      }
      id="description"
      autoFocus
      placeholder={"describe an idea"}
      autoComplete="off"
      value={idea.description}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === "Escape") e.target.blur();
      }}
      onChange={(e) =>
        dispatch({
          type: CHANGE_IDEA_DESCRIPTION,
          payload: e.target.value,
        })
      }
      onBlur={(e) => {
        if (idea.id) dispatch(changeIdeaDescription(idea.id, e.target.value));
      }}
    />
  );
};
