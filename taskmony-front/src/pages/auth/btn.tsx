type BtnProps = {
    label: string;
    onClick: Function;
    style?: string
  };

  export const Btn = ({ label, onClick }: BtnProps) => {
    return (
      <div className={"gap-4 flex p-2 mt-4 mb-2 w-full justify-center bg-blue-500 rounded-lg cursor-pointer"} onClick={()=>onClick()}>
        <span className={"text-white"}>{label}</span>
      </div>
    );
  };