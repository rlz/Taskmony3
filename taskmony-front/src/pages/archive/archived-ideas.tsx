import { useState } from "react";
import { useLocation } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import hrLine from "../../images/hr-line.svg";

export const ArchivedIdeas = () => {
    return (
      <div className="flex w-full">
        <div className="w-3/4    m-3 ml-0">
          <ArchivedItem label={"idea #2"} direction="Taskmony" />
          <ArchivedItem label={"idea #3"} direction="Taskmony" />
        </div>
        <Filter />
      </div>
    );
  }

  function Filter() {
    const [isOpen1, setIsOpen1] = useState<boolean>(true);
    const [isOpen2, setIsOpen2] = useState<boolean>(true);
    return (
      <div className="w-1/5 mt-4">
        <FilterDivider
          isOpen={isOpen1}
          setIsOpen={setIsOpen1}
          title="filter by deletion date"
        />
        {isOpen1 && (
          <div className="flex gap-2 mb-2">
          <input type="date" className="border border-gray-300 rounded pl-2 pr-2 font-semibold text-sm text-gray-800"/>
          <img src={hrLine}/>
          <input type="date" className="border border-gray-300 rounded pl-2 pr-2 font-semibold text-sm text-gray-800"/>
          </div>
        )}
        <hr />
        <FilterDivider
          isOpen={isOpen2}
          setIsOpen={setIsOpen2}
          title="filter by direction"
        />
        {isOpen2 && (
          <>
            <FilterItem label="Project #1" checked />
            <FilterItem label="Project #1" checked />
            <FilterItem label="Project #1" checked />
          </>
        )}
      </div>
    );
  }