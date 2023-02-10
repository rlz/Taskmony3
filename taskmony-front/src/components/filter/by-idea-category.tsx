import { useState } from "react";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const  FilterByIdeaCategory = () => {
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
            <FilterItem label="hot" checked radio/>
            <FilterItem label="later" checked={false} radio/>
            <FilterItem label="too good to delete" checked={false} radio/>
          </>
        )}
        </>
    );
  }