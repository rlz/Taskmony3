import { useState } from "react";
import { useLocation, useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/task-idea/archived-item";
import { FilterByDate } from "../../components/other-components/filter/by-date";
import { FilterByDirection } from "../../components/other-components/filter/by-direction";
import { useAppSelector } from "../../utils/hooks";

export const ArchivedIdeas = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const startDate = searchParams.get("startDate");
  const endDate = searchParams.get("endDate");
  const chosenDirection = searchParams.getAll("direction");
  const ideas = useAppSelector((store) => store.ideas.items);
  let chosenIdeas = ideas.filter(
    (i) => i.deletedAt != null
  );
  if(startDate){
    chosenIdeas = chosenIdeas.filter(
      (i) => i.deletedAt > startDate)
  }
  if(endDate){
    chosenIdeas = chosenIdeas.filter(
      (i) => i.deletedAt < endDate)
  }
  if (chosenDirection.length > 0)
  chosenIdeas = chosenIdeas.filter(
    (i) =>
      chosenDirection.includes(i.direction?.name) ||
      (chosenDirection.includes("unassigned") && !i.direction)
  );
  return (
    <div className="flex w-full">
      <div className="w-full    m-3 ml-0 mainBody">
      {chosenIdeas.map((idea,index)=><ArchivedItem             
      label={idea.description}
            date={idea.deletedAt}
            direction={idea.direction?.name}
            key={index} />)}
      </div>
      <Filter />
    </div>
  );
};

function Filter() {
  return (
    <div className="w-1/5 mt-4 filter">
      <FilterByDate type="deletion" />
      <hr />
      <FilterByDirection />
    </div>
  );
}
