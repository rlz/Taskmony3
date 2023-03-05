import { useEffect, useState } from "react";
import { BooleanParam, StringParam, useQueryParam } from "use-query-params";
import { getCookie } from "../../utils/cookies";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const FilterByFollowed = () => {
  const myId = getCookie("id");
  const [followed, setFollowed] = useQueryParam("followed",BooleanParam);

  return (
      <FilterItem
        label="show followed"
        checked={followed}
        onChange={(value, label) => {
          setFollowed(value)
          }}
      />
  );
};


export const FilterByIdeaCategory = () => {
  const [category, setCategory] = useQueryParam("ideaCategory", StringParam);
  const [isOpen, setIsOpen] = useState<boolean>(true);
  return (
    <>
      <FilterDivider
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        title="filter by category"
      />
      {isOpen && (
        <>
          <FilterItem
            label="hot"
            checked={category == "hot"}
            radio
            onChange={(value, label) => setCategory(label)}
          />
          <FilterItem
            label="later"
            checked={category == "later"}
            radio
            onChange={(value, label) => setCategory(label)}
          />
          <FilterItem
            label="too good to delete"
            checked={category == "too good to delete"}
            radio
            onChange={(value, label) => setCategory(label)}
          />
        </>
      )}
    </>
  );
};
