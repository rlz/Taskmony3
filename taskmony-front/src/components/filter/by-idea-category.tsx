import { useEffect, useState } from "react";
import { StringParam, useQueryParam } from "use-query-params";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

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
