import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { getCookie } from "../../utils/cookies";
import { useAppSelector } from "../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const FilterByCreator = ({ id }) => {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  let [searchParams, setSearchParams] = useSearchParams();
  const creator = searchParams.get("creator");
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((d) => d.id == id)[0];
  console.log(direction);
  const myId = getCookie("id");
  const users = direction?.members;
  return (
    <>
      <FilterDivider
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        title="filter by creator"
      />
      {isOpen && (
        <>
          <FilterItem label="all" checked />
          <FilterItem label="me" checked />
          {users?.map((u) => {
            u.id == myId ? (
              <FilterItem label={"me"} checked />
            ) : (
              <FilterItem
                label={u.displayName}
                checked={creator == u.name}
                onChange={(value, label) => {
                  if (value) setSearchParams({ creator: label });
                  else setSearchParams({ creator: "" });
                }}
              />
            );
          })}
        </>
      )}
    </>
  );
};
