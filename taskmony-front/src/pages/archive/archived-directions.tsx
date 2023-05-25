import { useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/task-idea/archived-item";
import { FilterByDate } from "../../components/other-components/filter/by-date";
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
      <div className="w-full  m-3 ml-0 mainBody">
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
    <div className="w-1/5 mt-4 filter">
      <FilterByDate type="deletion" />
    </div>
  );
}
