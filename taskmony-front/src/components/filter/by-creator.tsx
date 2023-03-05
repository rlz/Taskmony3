import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { ArrayParam, useQueryParam, withDefault } from "use-query-params";
import { getCookie } from "../../utils/cookies";
import { useAppSelector } from "../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const FilterByCreator = ({ id }) => {
  const MyFiltersParam = withDefault(ArrayParam, []);
  const [createdBy, setCreatedBy] = useQueryParam("createdBy", MyFiltersParam);
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((d) => d.id == id)[0];
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
          {users?.map((u) => (
            <FilterItem
              key={u.id}
              label={u.id == myId? "me" : u.displayName}
              checked={createdBy.includes(u.id)}
              onChange={(value, label) => {
                if (value) {
                  setCreatedBy([...createdBy, u.id]);
                } else {
                  setCreatedBy(createdBy.filter((el) => el != u.id));
                }
              }}
            />
          ))}
        </>
      )}
    </>
  );
};
