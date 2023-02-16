import { useState } from "react";
import { useLocation } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import hrLine from "../../images/hr-line.svg";
import { useAppSelector } from "../../utils/hooks";

export const ArchivedDirections = () => {
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt != null
  );
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3 ml-0">
        {directions.map((dir) => (
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
