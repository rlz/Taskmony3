import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const FilterByDirection = () => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  let [searchParams, setSearchParams] = useSearchParams();
  const chosenDirection = searchParams.get("direction");
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt == null
  );
  return (
    <>
      <FilterDivider
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        title="filter by direction"
      />
      {isOpen && (
        <>
          <FilterItem
            label={"none"}
            key={"0"}
            checked={chosenDirection == "none"}
            onChange={(value, label) => {
              if (value) setSearchParams({ direction: label });
              else setSearchParams({ direction: "" });
            }}
          />
          {directions.map((direction) => (
            <FilterItem
              label={direction.name}
              key={direction.id}
              checked={chosenDirection == direction.name}
              onChange={(value, label) => {
                if (value) setSearchParams({ direction: label });
                else setSearchParams({ direction: "" });
              }}
            />
          ))}
        </>
      )}
    </>
  );
};
