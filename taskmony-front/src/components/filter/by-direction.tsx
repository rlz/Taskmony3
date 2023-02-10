import { useState } from "react";
import { useAppSelector } from "../../utils/hooks";
import { FilterDivider } from "./filter-divider";
import { FilterItem } from "./filter-item";

export const  FilterByDirection = () => {
    const [isOpen, setIsOpen] = useState<boolean>(true);
    const directions = useAppSelector((store) => store.directions.items);
    return (
        <>
        <FilterDivider
          isOpen={isOpen}
          setIsOpen={setIsOpen}
          title="filter by direction"
        />
        {isOpen && (
          <>
          <FilterItem label={"none"} checked key={"0"} />
          {directions.map(direction =>
                        <FilterItem label={direction.name} key={direction.id} checked />
                        )}

          </>
        )}
      </>
    );
  }