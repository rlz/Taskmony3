import deleteI from "../../images/delete.svg";
import edit from "../../images/edit.svg";
import { AddBtn2 } from "../../components/add-btn/add-btn2";
import { FilterDivider } from "../../components/filter/filter-divider";
import { useEffect, useState } from "react";
import { AddUserModal } from "../../components/add-user-modal/add-user-modal";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { getCookie } from "../../utils/cookies";
import { changeDetails } from "../../services/actions/directionsAPI";

export const About = ({ directionId }) => {
  const dispatch = useAppDispatch();
  const myId = getCookie("id");
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((d) => d.id == directionId)[0];
  const [about, setAbout] = useState("");
  const users = direction?.members;
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  useEffect(()=>{
  setAbout(direction?.details)
  },[direction])

  const saveDescription = () => dispatch(changeDetails(about,directionId));
  return (
    <div>
      {isModalOpen && <AddUserModal close={() => setIsModalOpen(false)} />}
      <div className="flex justify-end">
        <LeaveBtn onClick={() => {}} />
      </div>
      <AboutElement value={about} onChange={setAbout} saveDescription={saveDescription}/>
      <FilterDivider isOpen={isOpen} setIsOpen={setIsOpen} title="USERS:" />
      {isOpen && (
        <>
          {users?.map((user) => (
            <User label={user.displayName} />
          ))}
        </>
      )}
      <div className="object-center w-full">
        <AddBtn2
          label="add a new user"
          style="justify-center"
          onClick={() => setIsModalOpen(true)}
        />
      </div>
    </div>
  );
};

type UserPropsT = {
  label: string;
};

export const User = ({ label }: UserPropsT) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <img src={deleteI} className="cursor-pointer"></img>
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
      </div>
    </div>
  );
};

export const AboutElement = ({ value, onChange, saveDescription }) => {
  const success = useAppSelector(store=>store.directions.setDetailsSuccess);
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm">
      <textarea
        className={
          "text-sm w-full focus:outline-none p-2 pr-4 pl-4 resize-none"
        }
        value={value}
        placeholder={"description"}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={(e)=> {
          if(e.key !== 'Enter') return;
          saveDescription(value)
        }}
      />
    </div>
  );
};

type LeaveBtnPropsT = {
  onClick: Function;
};

export const LeaveBtn = ({ onClick }: LeaveBtnPropsT) => {
  return (
    <div
      className={"p-1 w-fit mt-4 mb-2 pl-2 pr-2 bg-blue-400 rounded-lg"}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>Leave direction</span>
    </div>
  );
};
