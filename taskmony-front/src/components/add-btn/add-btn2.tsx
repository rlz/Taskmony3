import addCircle from "../../images/add_circle.svg";

type BtnProps = {
  label: string;
  onClick: Function;
  style?: string;
};

export const AddBtn2 = ({ label, onClick, style }: BtnProps) => {
  return (
    <div
      className={"gap-4 flex m-4 cursor-pointer " + style}
      onClick={() => onClick()}
    >
      <img src={addCircle}></img>
      <p className={"font-semibold text-sm text-gray-800"}>{label}</p>
    </div>
  );
};
