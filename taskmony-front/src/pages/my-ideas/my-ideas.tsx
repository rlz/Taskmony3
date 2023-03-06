import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { AddBtn } from "../../components/add-btn/add-btn";
import { EditedIdea } from "../../components/edited/edited-idea";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterByFollowed, FilterByIdeaCategory } from "../../components/filter/by-idea-category";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea";
import { addIdea, RESET_IDEA } from "../../services/actions/ideasAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";

function MyIdeas() {
  const [newIdea, setNewIdea] = useState(false);
  let [searchParams, setSearchParams] = useSearchParams();
  const chosenDirection = searchParams.getAll("direction");
  const chosenGeneration = searchParams.get("ideaCategory");
  const followed = searchParams.get("followed");
  const idea = useAppSelector((store) => store.editedIdea);
  let ideasToShow = useAppSelector((store) => store.ideas.items).filter(
    (i) => i.deletedAt == null
  );
  if (chosenDirection.length > 0)
    ideasToShow = ideasToShow.filter(
      (i) =>
        chosenDirection.includes(i.direction?.name) ||
        (chosenDirection.includes("unassigned") && !i.direction)
    );
    if (chosenGeneration){
      ideasToShow = ideasToShow.filter(
        (i) => i.generation === chosenGeneration
      );
    }

  const dispatch = useAppDispatch();
  const addANewIdea = () => {
    dispatch(addIdea(idea, null));
    dispatch({ type: RESET_IDEA });
  };
  const ideas = ideasToShow.map((idea,i) => <Idea last={i + 1 === ideasToShow.length} idea={idea} key={idea.id} direction={undefined} />);
  return (
    <div className="flex w-full">
      <div className="w-3/4  p-3 flex flex-col overflow-hidden h-screen">
        <h1 className="font-bold text-3xl">My Ideas</h1>
        <AddBtn label={"add a new idea"} onClick={() => setNewIdea(true)} />
        {newIdea && (
          <EditedIdea
            label={"new idea"}
            save={() => {
              console.log("saving a new idea");
              setNewIdea(false);
              addANewIdea();
            }}
            close={() => {
              setNewIdea(false);
            }}
          />
        )}
        <div className="overflow-x-hidden">{ideas}</div>
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  return (
    <div className="w-1/5 mt-12">
      <FilterByIdeaCategory />
      <hr />
      <FilterByFollowed />
      <hr />
      <FilterByDirection />
    </div>
  );
}

export default MyIdeas;
