import { useState } from "react";
import { useLocation, useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import hrLine from "../../images/hr-line.svg";
import { useAppSelector } from "../../utils/hooks";

export const ArchivedDirections = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const startDate = searchParams.get("startDate");
  const endDate = searchParams.get("endDate");
  const directions = useAppSelector((store) => store.directions.items);
  let chosenDirections = directions.filter(
    (i) => i.deletedAt != null
  );
  if(startDate){
    chosenDirections = chosenDirections.filter(
      (i) => i.deletedAt > startDate)
  }
  if(endDate){
    chosenDirections = chosenDirections.filter(
      (i) => i.deletedAt < endDate)
  }
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3 ml-0">
        {chosenDirections.map((dir) => (
          <ArchivedItem label={dir.name} date={dir.deletedAt} key={dir.id} />
        ))}
      </div>
      <Filter />
    </div>
  );
};

function Filter() {
  return (
    <div className="w-1/5 mt-4">
      <FilterByDate type="deletion" />
    </div>
  );
}
