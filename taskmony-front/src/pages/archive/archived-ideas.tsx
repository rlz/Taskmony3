import { useState } from "react";
import { useLocation } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterByDirection } from "../../components/filter/by-direction";
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
    return (
      <div className="w-1/5 mt-4">
          <FilterByDate type="deletion"/>
        <hr />
        <FilterByDirection/>
      </div>
    );
  }