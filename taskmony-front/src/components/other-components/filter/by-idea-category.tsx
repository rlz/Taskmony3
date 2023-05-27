import { useState } from "react";
import { BooleanParam, StringParam, useQueryParam } from "use-query-params";
import Cookies from "js-cookie";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const FilterByFollowed = () => {
  const myId = Cookies.get("id");
  const [followed, setFollowed] = useQueryParam("followed", BooleanParam);

  return (
    <FilterItem
      label="show followed"
      checked={followed ? followed : false}
      onChange={(value: boolean, label: string) => {
        setFollowed(value);
      }}
    />
  );
};

export const FilterByIdeaCategory = () => {
  const [category, setCategory] = useQueryParam("ideaCategory", StringParam);
  if (!category) setCategory("HOT");
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
            onChange={(value: boolean, label: string) => setCategory("HOT")}
          />
          <FilterItem
            label="later"
            checked={category == "LATER"}
            radio
            onChange={(value: boolean, label: string) => setCategory("LATER")}
          />
          <FilterItem
            label="too good to delete"
            checked={category == "TOO_GOOD_TO_DELETE"}
            radio
            onChange={(value: boolean, label: string) =>
              setCategory("TOO_GOOD_TO_DELETE")
            }
          />
        </>
      )}
    </>
  );
};
