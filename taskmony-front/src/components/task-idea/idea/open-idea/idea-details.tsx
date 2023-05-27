import {
  changeIdeaDirection,
  changeIdeaGeneration,
  CHANGE_IDEA_DIRECTION,
  CHANGE_IDEA_GENERATION,
} from "../../../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";
import { ItemPicker } from "../../open-items-components/item-picker";

type DetailsProps = { fromDirection?: string | null };

export const Details = ({ fromDirection }: DetailsProps) => {
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
