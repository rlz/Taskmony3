import { useState } from "react";
import { ArrayParam, useQueryParam, withDefault } from "use-query-params";
import Cookies from 'js-cookie';
import { useAppSelector } from "../../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

type FilterByCreatorProps = {
  id: string;
}
export const FilterByCreator = ({ id } : FilterByCreatorProps) => {
  const MyFiltersParam = withDefault(ArrayParam, []);
  const [createdBy, setCreatedBy] = useQueryParam("createdBy", MyFiltersParam);
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((d) => d.id == id)[0];
  const myId = Cookies.get("id");
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
              onChange={(value : boolean, label : string) => {
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