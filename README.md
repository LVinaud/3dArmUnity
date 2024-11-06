# 3dArmUnity
This project is based on [N-AxisRoboticArmControl](https://github.com/brenocq/N-AxisRoboticArmControl/tree/master), which is licensed under the MIT License. Significant modifications have been made by Lázaro Vinaud as part of this research project.

# Active Goals

Adding the interface to allow user creating of scenarios and configurations

Making the distance to obstacle more efficient just as the 2D by making a weighted grid.

# Achieved Goals

Making a process for the arm to go from a later best configuration to the current best one in a controlled way in order to avoid strange jumps in evolution, done by using the maxStep variable. ✅

Making the path finding script work more smoothly visually, as it is currently crashing. ✅(the a* is not visualized anymore)

Making it possible for the simulation run a limited amount of generations and saving data about it, the fitness evolution along with other aspects. ✅

Adding an already existing Unity A* method, just as the 2d project done pastly. ✅
