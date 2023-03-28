import { useEffect, useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import {
  FilterByFollowed,
  FilterByIdeaCategory,
} from "../../components/filter/by-idea-category";
import { Idea } from "../../components/idea";
import { FilterDivider } from "../../components/filter/filter-divider";
import { EditedIdea } from "../../components/edited/edited-idea";
import {
  addIdea,
  CHANGE_IDEA_DIRECTION,
  RESET_IDEA,
} from "../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { FilterByCreator } from "../../components/filter/by-creator";
import { useSearchParams } from "react-router-dom";

type IdeasProps = {
  directionId: string; directionName: string
}

function Ideas({ directionId, directionName } : IdeasProps) {

  const [newIdea, setNewIdea] = useState(false);
  let [searchParams, setSearchParams] = useSearchParams();
  const createdBy = searchParams.getAll("createdBy");
  const chosenGeneration = searchParams.get("ideaCategory");
  const followed = searchParams.get("followed");
  const idea = useAppSelector((store) => store.editedIdea);
  useEffect(()=>{
  if(idea.id !== "") setNewIdea(false);
  },[idea.id])
  let ideasToShow = useAppSelector((store) => store.ideas.items).filter(
    (i) => i.deletedAt == null && i.direction?.id == directionId
  ).sort((a, b) => {
    //console.log("sorting")
    if(!a.reviewedAt && b.reviewedAt) return -1
    else if(!b.reviewedAt && a.reviewedAt) return 1
    else if(!b.reviewedAt && !a.reviewedAt) return 0
    else {return b.reviewedAt < a.reviewedAt?1:-1 }
  });;

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
    (d) => d.id == directionId
  )[0];
  const addANewIdea = (direction : string) => {
    dispatch(addIdea(idea, direction));
    dispatch({ type: RESET_IDEA });
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
      <div className="w-full  m-3">
        <AddBtn
          label={"add a new idea"}
          onClick={() => {
            dispatch({ type: RESET_IDEA })
            setNewIdea(true);
            dispatch({
              type: CHANGE_IDEA_DIRECTION,
              payload: direction,
            });
          }}
        />
        {newIdea && idea.id === "" && (
          <EditedIdea
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
}

function Filter({ directionId } : FilterProps) {
  return (
    <div className="w-1/5 mt-4 filter">
      <FilterByIdeaCategory />
      <hr />
      <FilterByCreator id={directionId} />
    </div>
  );
}

export default Ideas;
