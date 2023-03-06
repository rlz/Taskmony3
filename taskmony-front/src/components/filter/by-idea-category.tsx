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
            checked={category == "HOT"}
            radio
            onChange={(value, label) => setCategory("HOT")}
          />
          <FilterItem
            label="later"
            checked={category == "LATER"}
            radio
            onChange={(value, label) => setCategory("LATER")}
          />
          <FilterItem
            label="too good to delete"
            checked={category == "TOO_GOOD_TO_DELETE"}
            radio
            onChange={(value, label) => setCategory("TOO_GOOD_TO_DELETE")}
          />
        </>
      )}
    </>
  );
};
