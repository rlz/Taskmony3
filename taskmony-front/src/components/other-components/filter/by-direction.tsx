import { useState } from "react";
import { useAppSelector } from "../../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";
import { useQueryParam, ArrayParam, withDefault } from "use-query-params";

export const FilterByDirection = () => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const MyFiltersParam = withDefault(ArrayParam, []);
  const [dir, setDir] = useQueryParam("direction", MyFiltersParam);
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
            label={"unassigned"}
            style={"italic"}
            key={"0"}
            checked={dir.includes("unassigned")}
            onChange={(value: string, label: string) => {
              if (value) {
                setDir([...dir, "unassigned"]);
              } else {
                setDir(dir.filter((el) => el != "unassigned"));
              }
            }}
          />
          {directions.map((direction) => (
            <FilterItem
              label={direction.name}
              key={direction.id}
              checked={dir.includes(direction.name)}
              onChange={(value: string, label: string) => {
                if (value) {
                  setDir([...dir, label]);
                } else {
                  setDir(dir.filter((el) => el != label));
                }
              }}
            />
          ))}
        </>
      )}
    </>
  );
};
