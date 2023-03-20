import { useState } from "react";
import { FilterDivider } from "./filter-divider";
import hrLine from "../../images/hr-line.svg";
import { FilterItem } from "./filter-item";
import { useSearchParams } from "react-router-dom";
import Cookies from 'js-cookie';
import {
  useQueryParam,
  NumberParam,
  StringParam,
  ArrayParam,
  withDefault,
  BooleanParam,
} from "use-query-params";

export const FilterByTaskType = () => {
  const myId = Cookies.get("id");
  const [future, setFuture] = useQueryParam("future",BooleanParam);
  const [followed, setFollowed] = useQueryParam("followed",BooleanParam);
  const [assignedByMe,setAssignedByMe] = useQueryParam("assignedByMe",BooleanParam);

  return (
    <>
      <FilterItem
        label="show future tasks"
        checked={future?future:false}
        onChange={(value : boolean, label : string) => {
          setFuture(value)
          }}
      />
      <FilterItem
        label="show assigned by me"
        checked={assignedByMe?assignedByMe:false}
        onChange={(value : boolean, label : string) => {
          setAssignedByMe(value)
        }}
      />
      <FilterItem
        label="show followed"
        checked={followed?followed:false}
        onChange={(value : boolean, label : string) => {
          setFollowed(value)
          }}
      />
    </>
  );
};

export const FilterByFuture = () => {
  const [future, setFuture] = useQueryParam("future",BooleanParam);

  return (
    <>
      <FilterItem
        label="show future tasks"
        checked={future?future:false}
        onChange={(value : boolean, label : string) => {
          setFuture(value)
          }}
      />
    </>
  );
};

export const FilterByArchivedTaskType = () => {
  const [archiveType, setArchiveType] = useQueryParam("archiveType",StringParam);
  if(!archiveType) setArchiveType("completed")
  return (
    <>
      <FilterItem
        label="deleted"
        checked={archiveType == "deleted"}
        onChange={(value : boolean, label : string) => {
          if (value) setArchiveType("deleted") 
          else setArchiveType("completed")
        }}
        radio
      />
      <FilterItem
        label="completed"
        checked={archiveType == "completed"}
        onChange={(value : boolean, label : string) => {
          if (value) setArchiveType("completed")
          else setArchiveType("deleted")
        }}
        radio
      />
    </>
  );
};
