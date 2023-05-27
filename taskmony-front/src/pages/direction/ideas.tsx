import { useEffect, useState } from "react";
import { AddBtn } from "../../components/other-components/buttons/add-btn";
import { FilterByIdeaCategory } from "../../components/other-components/filter/by-idea-category";
import { Idea } from "../../components/task-idea/idea/idea";
import { OpenIdea } from "../../components/task-idea/idea/open-idea/open-idea";
import {
  addIdea,
  CHANGE_IDEA_DIRECTION,
  RESET_IDEA,
} from "../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { FilterByCreator } from "../../components/other-components/filter/by-creator";
import { useSearchParams } from "react-router-dom";

type IdeasProps = {
  directionId: string;
  directionName: string;
};

function Ideas({ directionId, directionName }: IdeasProps) {
  const [newIdea, setNewIdea] = useState(false);
  let [searchParams] = useSearchParams();
  const createdBy = searchParams.getAll("createdBy");
  const chosenGeneration = searchParams.get("ideaCategory");
  const idea = useAppSelector((store) => store.editedIdea);
  useEffect(() => {
    if (idea.id !== "") setNewIdea(false);
  }, [idea.id]);
  let ideasToShow = useAppSelector((store) => store.ideas.items)
    .filter((i) => i.deletedAt == null && i.direction?.id === directionId)
    .sort((a, b) => {
      //console.log("sorting")
      if (b.reviewedAt == null && a.reviewedAt == null) return 0;
      else if (a.reviewedAt == null) return -1;
      else if (b.reviewedAt == null) return 1;
      else {
        return b.reviewedAt < a.reviewedAt ? 1 : -1;
      }
    });

  if (createdBy.length > 0) {
    ideasToShow = ideasToShow.filter((i) => createdBy.includes(i.createdBy.id));
  }
  if (createdBy.length > 0)
    ideasToShow = ideasToShow.filter((i) => createdBy.includes(i.createdBy.id));
  if (chosenGeneration) {
    ideasToShow = ideasToShow.filter((i) => i.generation === chosenGeneration);
  }

  const dispatch = useAppDispatch();
  const direction = useAppSelector((store) => store.directions.items).filter(
    (d) => d.id === directionId
  )[0];
  const addANewIdea = (direction: string) => {
    dispatch(addIdea(idea, direction));
    // dispatch({ type: RESET_IDEA });
  };
  const ideas = ideasToShow.map((idea, i) => (
    <Idea
      last={i + 1 === ideasToShow.length}
      idea={idea}
      direction={directionName}
      key={idea.id}
    />
  ));
  return (
    <div className="flex w-full">
      <div className="w-full  m-3 mainBody">
        <AddBtn
          label={"add a new idea"}
          onClick={() => {
            dispatch({ type: RESET_IDEA });
            setNewIdea(true);
            dispatch({
              type: CHANGE_IDEA_DIRECTION,
              payload: direction,
            });
          }}
        />
        {newIdea && idea.id === "" && (
          <OpenIdea
            label={"new idea"}
            direction={directionName}
            save={() => {
              //console.log("saving a new idea");
              setNewIdea(false);
              addANewIdea(directionId);
            }}
            close={() => {
              setNewIdea(false);
            }}
          />
        )}
        {ideas}
      </div>
      <Filter directionId={directionId} />
    </div>
  );
}
type FilterProps = {
  directionId: string;
};

function Filter({ directionId }: FilterProps) {
  return (
    <div className="w-1/5 mt-4 filter">
      <FilterByIdeaCategory />
      <hr />
      <FilterByCreator id={directionId} />
    </div>
  );
}

export default Ideas;
