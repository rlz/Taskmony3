import { useState } from "react";
import { FilterDivider } from "./filter-divider";
import hrLine from "../../images/hr-line.svg";

export const FilterByDate = ({type}) => {
    const [isOpen, setIsOpen] = useState<boolean>(true);
    return (
      <>
        <FilterDivider
          isOpen={isOpen}
          setIsOpen={setIsOpen}
          title={`filter by ${type} date`}
        />
        {isOpen && (
          <div className="flex gap-2 mb-2">
          <input type="date" className="border border-gray-300 rounded pl-2 pr-2 font-semibold text-sm text-gray-800"/>
          <img src={hrLine}/>
          <input type="date" className="border border-gray-300 rounded pl-2 pr-2 font-semibold text-sm text-gray-800"/>
          </div>
        )}

        </>
    );
  }