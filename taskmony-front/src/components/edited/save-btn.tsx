import add from "../../images/add-white.svg";

type BtnProps = {
  label: string;
  onClick: Function;
};

export const SaveBtn = ({ label, onClick }: BtnProps) => {
  return (
    <div className={"w-full flex justify-end"} >
    <div className={"w-auto inline-block p-1 m-1 mr-2 bg-blue-500 rounded-lg cursor-pointer"} onClick={()=>onClick()}>
      <span className={"text-white p-1 pr-2 pl-2"}>{label}</span>
    </div>
    </div>
  );
};
