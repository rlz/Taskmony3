import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { AddBtn } from "../components/other-components/buttons/add-btn";
import { OpenIdea } from "../components/task-idea/idea/open-idea/open-idea";
import { FilterByDirection } from "../components/other-components/filter/by-direction";
import {
  FilterByFollowed,
  FilterByIdeaCategory,
} from "../components/other-components/filter/by-idea-category";

import { Idea } from "../components/task-idea/idea/idea";
import { addIdea, RESET_IDEA } from "../services/actions/ideasAPI";
import Cookies from "js-cookie";
import { useAppDispatch, useAppSelector } from "../utils/hooks";

function MyIdeas() {
  const [newIdea, setNewIdea] = useState(false);
  const myId = Cookies.get("id");
  let [searchParams] = useSearchParams();
  const chosenDirection = searchParams.getAll("direction");
  const chosenGeneration = searchParams.get("ideaCategory");
  const followed = searchParams.get("followed");
  const idea = useAppSelector((store) => store.editedIdea);
  useEffect(() => {
    if (idea.id !== "") setNewIdea(false);
  }, [idea.id]);
  let ideasToShow = useAppSelector((store) => store.ideas.items)
    .filter((i) => i.deletedAt == null)
    .filter(
      (i) => i.createdBy.id === myId || i.subscribers.some((s) => s.id === myId)
    )
    .sort((a, b) => {
      //console.log("sorting")
      if (b.reviewedAt == null && a.reviewedAt == null) return 0;
      else if (a.reviewedAt == null) return -1;
      else if (b.reviewedAt == null) return 1;
      else {
        return b.reviewedAt < a.reviewedAt ? 1 : -1;
      }
    });
  if (chosenDirection.length > 0)
    ideasToShow = ideasToShow.filter(
      (i) =>
        chosenDirection.includes(i.direction?.name) ||
        (chosenDirection.includes("unassigned") && !i.direction)
    );
  if (chosenGeneration) {
    ideasToShow = ideasToShow.filter((i) => i.generation === chosenGeneration);
  }
  if (!followed || followed === "0") {
    ideasToShow = ideasToShow.filter(
      (i) =>
        !(i.subscribers.some((s) => s.id === myId) && i.createdBy.id !== myId)
    );
  }

  const dispatch = useAppDispatch();
  const addANewIdea = () => {
    dispatch(addIdea(idea, null));
    // dispatch({ type: RESET_IDEA });
  };
  const ideas = ideasToShow.map((idea, i) => (
    <Idea
      last={i + 1 === ideasToShow.length}
      idea={idea}
      key={idea.id}
      direction={undefined}
    />
  ));
  return (
    <div className="flex w-full">
      <div className="w-full  p-3 flex flex-col overflow-hidden h-screen mainBody">
        <h1 className="font-bold text-3xl">My Ideas</h1>
        <AddBtn
          label={"add a new idea"}
          onClick={() => {
            dispatch({ type: RESET_IDEA });
            setNewIdea(true);
          }}
        />
        {newIdea && idea.id === "" && (
          <OpenIdea
            label={"new idea"}
            save={() => {
              //console.log("saving a new idea");
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
    <div className="w-1/5 mt-12 filter">
      <FilterByIdeaCategory />
      <hr />
      <FilterByFollowed />
      <hr />
      <FilterByDirection />
    </div>
  );
}

export default MyIdeas;
