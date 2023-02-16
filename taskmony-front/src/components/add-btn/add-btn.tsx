import add from "../../images/add-white.svg";

type BtnProps = {
  label: string;
  onClick: Function;
};

export const AddBtn = ({ label, onClick }: BtnProps) => {
  return (
    <div
      className={
        "gap-4 flex p-2 mt-4 mb-2 w-full justify-center bg-blue-500 rounded-lg cursor-pointer"
      }
      onClick={() => onClick()}
    >
      <img src={add}></img>
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
