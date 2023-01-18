import deleteI from "../../images/delete.svg";
import edit from "../../images/edit.svg";
import { AddBtn2 } from "../../components/add-btn/add-btn2";
import { FilterDivider } from "../../components/filter/filter-divider";
import { useState } from "react";
import { AddUserModal } from "../../components/add-user-modal/add-user-modal";


export const About = () => {

  const text = `Facit igitur Lucius noster prudenter, qui audire de summo bono potissimum velit;
    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Haeret in salebra. Invidiosum nomen est, infame, suspectum.
    An hoc usque quaque, aliter in vita?`;
    const [isOpen, setIsOpen] = useState<boolean>(true);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  return (
    <div>
      {isModalOpen && <AddUserModal close={()=>setIsModalOpen(false)}/>}
      <div className="flex justify-end"><LeaveBtn onClick={()=>{}}/></div>
      <AboutElement text={text}/>
      <FilterDivider isOpen={isOpen} setIsOpen={setIsOpen} title="USERS:" />
      {isOpen && (
        <>
          <User label="John Doe"/>
          <User label="Jack Green"/>
          <User label="Ann Smith"/>
        </>
      )}
      <div className="object-center w-full">
        <AddBtn2 label="add a new user" style="justify-center" onClick={()=>setIsModalOpen(true)} />
      </div>
    </div>
  );
};

type UserPropsT = {
label: string;
}

export const User = ({label}:UserPropsT) => {
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

  type AboutPropsT = {
    text: string;
    }

  export const AboutElement = ({text}:AboutPropsT) => {
    return (
        <div className="w-full bg-white rounded-lg drop-shadow-sm">
        <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
          <div className="flex  gap-2">
            <span className={"text-sm"}>{text}</span>
            <img src={edit} className="cursor-pointer"></img>
          </div>
        </div>
        </div>
    );
  };

  type LeaveBtnPropsT = {
    onClick: Function;
    }

  export const LeaveBtn = ({onClick}:LeaveBtnPropsT) => {
    return (
        <div className={"p-1 w-fit mt-4 mb-2 bg-blue-400 rounded-lg"} onClick={()=>onClick()}>
        <span className={"text-white"}>leave direction</span>
      </div>
    );
  };

