import { BigBtn } from "../big-btn"

const RepeatedModal = ({question,leftButtonLabel,leftClick,rightButtonLabel,rightClick}) => {
    return(
        <div className=" w-full h-full absolute top-0 left-0 bg-white z-40 rounded-lg flex items-center justify-center">
        <div>
        <p>{question}</p>
        <div className="mb-1 flex justify-end absolute bottom-0 right-0">
        <BigBtn label={leftButtonLabel} onClick={()=>{leftClick();}}/>
        <BigBtn label={rightButtonLabel} onClick={()=>{rightClick();}}/>
        </div>
        </div>
        </div>
    )
}

export const ChangeRepeatedValueModal = ({changeThis,changeAll}) => {
    return(
<RepeatedModal
question={"This is a repeated task. Do you want to change all tasks?"} 
leftButtonLabel={"change all"}
leftClick={changeAll}
rightButtonLabel={"change this task only"}
rightClick={changeThis}
/>
    )
}

export const DeleteRepeatedValueModal = ({deleteThis,deleteAll}) => {
    return(
<RepeatedModal
question={"This is a repeated task. Do you want to delete all tasks?"} 
leftButtonLabel={"delete all"}
leftClick={deleteAll}
rightButtonLabel={"delete this task only"}
rightClick={deleteThis}
/>
    )
}

export const ChangeRepeatedModeModal = () => {
    return(
<RepeatedModal
question={"Changing repeat mode or starting date of a repeated task may delete some tasks. Do you want to proceed?"} 
leftButtonLabel={"yes"}
leftClick={()=>{}}
rightButtonLabel={"cancel"}
rightClick={()=>{}}
/>
    )
}