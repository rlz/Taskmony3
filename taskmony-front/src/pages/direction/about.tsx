import deleteI from "../../images/delete.svg";
import edit from "../../images/edit.svg";
import { AddBtn2 } from "../../components/add-btn/add-btn2";
import { FilterDivider } from "../../components/filter/filter-divider";
import { useEffect, useState } from "react";
import { AddUserModal } from "../../components/add-user-modal/add-user-modal";
import useIsFirstRender, { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { getCookie } from "../../utils/cookies";
import { changeDetails, deleteDirection, removeUser, REMOVE_DIRECTION } from "../../services/actions/directionsAPI";
import { useNavigate } from "react-router-dom";

export const About = ({ directionId }) => {
  const dispatch = useAppDispatch();
  const myId = getCookie("id");
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((d) => d.id == directionId)[0];
  const [about, setAbout] = useState("");
  const users = direction?.members;
  const navigate = useNavigate();
  const isFirst = useIsFirstRender();
  const amIOwner = direction
  const [isOpen, setIsOpen] = useState<boolean>(true);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const deleted_success = useAppSelector((store) => store.directions.delete_direction_success);

  useEffect(()=>{
  setAbout(direction?.details);
  },[direction?.details])

  useEffect(()=>{
    if (!isFirst & deleted_success) navigate("/");
    },[deleted_success])

  // useEffect(()=>{
  //   if(direction.members.length == 0) leaveDirection();
  //   },[direction.members])
  
  const leaveDirection = () => {
  if(direction.members.length <=1) {deleteThisDirection();return;}
  //remove yourself 
  dispatch(removeUser(directionId,{id:myId}))
  //remove from list
  dispatch({
    type: REMOVE_DIRECTION,
    directionId:directionId,
  });
  //do to start page
  }
  const deleteThisDirection = () => {
  //setDeletedAt  
  dispatch(deleteDirection(directionId));
  //remove from list
  //do to start page
  }

  const saveDescription = () => dispatch(changeDetails(about,directionId));
  const removeThisUser = (user) => dispatch(removeUser(directionId,user))
  
  return (
    <div>
      {isModalOpen && <AddUserModal close={() => setIsModalOpen(false)} />}
      <div className="flex justify-end gap-2">
        <Btn onClick={leaveDirection} label="Leave direction" color="blue"/>
        <Btn onClick={deleteThisDirection} label="Delete direction" color="red"/>
      </div>
      <AboutElement value={about} onChange={setAbout} saveDescription={saveDescription}/>
      {users?.length > 1 && <FilterDivider isOpen={isOpen} setIsOpen={setIsOpen} title="USERS:" />}
      {isOpen && (
        <>
          {users?.map((user) => (
            myId == user.id? null :
            <User label={user.displayName} onClick={()=>removeThisUser(user)} />
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

export const User = ({ label, onClick }: UserPropsT) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm" onClick={onClick}>
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
        onBlur={(e)=> {
          saveDescription(value)
        }}
      />
    </div>
  );
};

type LeaveBtnPropsT = {
  onClick: Function;
};

const Btn = ({ onClick, label, color }) => {
  return (
    <div
      className={`p-1 w-fit mt-4 mb-2 pl-2 pr-2 bg-${color}-400 rounded-lg cursor-pointer`}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
