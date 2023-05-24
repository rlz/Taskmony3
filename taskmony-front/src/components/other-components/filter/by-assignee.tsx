import { useState } from "react";
import { ArrayParam, useQueryParam, withDefault } from "use-query-params";
import Cookies from 'js-cookie';
import { useAppSelector } from "../../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

type FilterByAssigneeProps = {
  id: string;
}
export const FilterByAssignee = ({ id } : FilterByAssigneeProps) => {
  const MyFiltersParam = withDefault(ArrayParam, []);
  const [assigned, setAssignedBy] = useQueryParam("assignedTo", MyFiltersParam);
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
        title="filter by assignee"
      />
      {isOpen && (
        <>
          {users?.map((u) => (
            <FilterItem
              key={u.id}
              label={u.id == myId? "me" : u.displayName}
              checked={assigned.includes(u.id)}
              onChange={(value : boolean, label : string) => {
                if (value) {
                  setAssignedBy([...assigned, u.id]);
                } else {
                  setAssignedBy(assigned.filter((el) => el != u.id));
                }
              }}
            />
          ))}
        </>
      )}
    </>
  );
};
