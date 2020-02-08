import numpy as np
from collections import deque

class Solution(object):
    def __init__(self, file_path):
        self.file_path = file_path
        self.H_vectors = np.array([[]])
        self.V_vectors = np.array([[]])
        self.V2_table = np.array([[]]) # all combinations of 2 vertical photos
        self.H2V_table = np.array([[]]) # interest factor adjacency matrix
        self.tag_index_dict = {}
        self.H_ID_mapping = [] # map H_vector index to photo ID
        self.V_ID_mapping = [] # map V_vector index to photo ID
        self.H2V_mapping = [] # index in H2V table maps to H or V2
        self.V2_mapping = [] # index in V2 table maps to tuple of 2 V's

    def load_data(self):
        f = open(self.file_path, 'r')
        N = int(f.readline())
        photo_ID = 0
        for line in f:
            line = line.strip()
            photo_info = line.split(' ')
            orientation = photo_info[0]
            num_tags = int(photo_info[1])
            tag_list = photo_info[2:]
            self.populate_structures(orientation, num_tags, tag_list, photo_ID)
            photo_ID = photo_ID + 1
        self.calc_interest_factor()
        self.bfs()

    def calc_interest_factor(self):
        # populate V2 table
        for index, row in enumerate(self.V_vectors):
            if index == np.size(self.V_vectors, axis=0) - 1:
                break
            bitwiseOR_2Vs = np.greater(np.add(self.V_vectors, row), 0).astype(int)[index + 1:]
            for v1 in range(index + 1):
                for v2 in range(index + 1, np.size(self.V_vectors, 0)):
                    self.V2_mapping.append((v1, v2))
            if np.size(self.V2_table, 1) == 0:
                self.V2_table = bitwiseOR_2Vs
            else:
                self.V2_table = np.vstack([self.V2_table, bitwiseOR_2Vs])
        # populate H2V table
        temp = np.vstack([self.H_vectors, self.V2_table])
        for h in range(len(self.H_vectors)):
            self.H2V_mapping.append(('H', h))
        if np.size(self.V2_table, 1) > 0:
            for v2 in range(np.size(self.V2_table, 0)):
                self.H2V_mapping.append(('V', v2))
        for index, row in enumerate(temp):
            tempSum = np.add(temp, row)
            interest_factor_1 = np.equal(tempSum, 2).astype(int)
            bitwiseOR_2slides = np.greater(tempSum, 0).astype(int)
            interest_factor_2 = np.subtract(bitwiseOR_2slides, row)
            interest_factor_3 = np.subtract(bitwiseOR_2slides, temp)
            interest_factor = np.minimum(np.sum(interest_factor_1, axis=1), \
                                         np.minimum(np.sum(interest_factor_2, axis=1), \
                                                    np.sum(interest_factor_3, axis=1)))
            if np.size(self.H2V_table, 1) == 0:
                self.H2V_table = np.reshape(interest_factor, (1, len(interest_factor)))
            else:
                self.H2V_table = np.vstack([self.H2V_table, interest_factor])

    def populate_structures(self, orientation, num_tags, tag_list, photo_ID):
        vector = np.zeros(len(self.tag_index_dict))
        for tag in tag_list:
            if not tag in self.tag_index_dict:
                self.tag_index_dict[tag] = len(self.tag_index_dict)
                vector = np.append(vector, 1)
            else:
                vector[self.tag_index_dict[tag]] = 1
        self.pad_with_zeros()
        if orientation == 'H':
            if np.size(self.H_vectors, 1) == 0:
                self.H_ID_mapping.append(photo_ID)
                self.H_vectors = np.reshape(vector, (1, len(vector)))
            else:
                self.H_ID_mapping.append(photo_ID)
                self.H_vectors = np.vstack([self.H_vectors, vector])
        if orientation == 'V':
            if np.size(self.V_vectors, 1) == 0:
                self.V_ID_mapping.append(photo_ID)
                self.V_vectors = np.reshape(vector, (1, len(vector)))
            else:
                self.V_ID_mapping.append(photo_ID)
                self.V_vectors = np.vstack([self.V_vectors, vector])               

    def pad_with_zeros(self):
        if np.size(self.H_vectors, 1) > 0 and \
        len(self.tag_index_dict) > np.size(self.H_vectors, 1):
            z = np.zeros((np.size(self.H_vectors, 0), \
                              len(self.tag_index_dict) - np.size(self.H_vectors, 1)))
            self.H_vectors = np.concatenate((self.H_vectors, z), axis=1)
        if np.size(self.V_vectors, 1) > 0 and \
        len(self.tag_index_dict) > np.size(self.V_vectors, 1):
            z = np.zeros((np.size(self.V_vectors, 0), \
                              len(self.tag_index_dict) - np.size(self.V_vectors, 1)))
            self.V_vectors = np.concatenate((self.V_vectors, z), axis=1)
        # maybe try appending and reshaping at the end?

    def bfs(self):
        overallMaxIndexList = []
        overallMaxSum = 0
        for i in range(np.size(self.H2V_table, 0)):
            obj = BFS_object()
            id_list = self.H2V_index_to_photoIDs(i)
            obj.add(i, id_list, 0)
            maxSum, maxIndexList = self._bfs(obj)
            if maxSum > overallMaxSum:
                overallMaxSum = maxSum
                overallMaxIndexList = maxIndexList
        self.print_solution(overallMaxSum, overallMaxIndexList)

    def _bfs(self, start_obj):
        q = deque([start_obj])
        maxIndexList = []
        maxSum = 0
        while len(q) > 0:
            current_obj = q.popleft()
            for i in range(np.size(self.H2V_table, 0)):
                current_index = current_obj.index_list[-1]
                current_photoIDs = self.H2V_index_to_photoIDs(current_index)
                i_photoIDs = self.H2V_index_to_photoIDs(i)
                if len(i_photoIDs.intersection(current_obj.id_set)) == 0:
                    new_obj = BFS_object(obj=current_obj)
                    new_obj.add(i, i_photoIDs, self.H2V_table[current_index][i])
                    if new_obj.interest_sum > maxSum:
                        maxSum = new_obj.interest_sum
                        maxIndexList = new_obj.index_list
                    q.append(new_obj)
        return maxSum, maxIndexList

    def H2V_index_to_photoIDs(self, H2V_index):
        orientation, index = self.H2V_mapping[H2V_index]
        ret_set = set()
        if orientation == 'H':
            ret_set.add(self.H_ID_mapping[index])
        else:
            idx1, idx2 = self.V2_mapping[index]
            ret_set.add(self.V_ID_mapping[idx1])
            ret_set.add(self.V_ID_mapping[idx2])
        return ret_set

    def print_solution(self, max_val, max_list):
        print('Total interest factor: ', max_val)
        print(len(max_list))
        for i in max_list:
            s = self.H2V_index_to_photoIDs(max_list[i])
            str1 = ''
            for x in s:
                str1 += str(x) + ' '
            print(str1.rstrip())

    def print(self):
        print('H_vectors', str(self.H_vectors))
        print('V_vectors', str(self.V_vectors))
        print('V2_table', str(self.V2_table))
        print('H2V_table', str(self.H2V_table))
        print('tag_index_dict', str(self.tag_index_dict))
        print('H_ID_mapping', str(self.H_ID_mapping))
        print('V_ID_mapping', str(self.V_ID_mapping))
        print('H2V_mapping', str(self.H2V_mapping))
        print('V2_mapping', str(self.V2_mapping))

class BFS_object():
    def __init__(self, index_list=deque([]), id_set=set(), interest_sum=0, obj=None):
        if obj == None:
            self.index_list = index_list
            self.id_set = id_set
            self.interest_sum = interest_sum
        else:
            self.index_list = deque(obj.index_list)
            self.id_set = set(obj.id_set)
            self.interest_sum = obj.interest_sum

    def add(self, idx, id_set, interest_factor):
        self.index_list.append(idx)
        self.id_set = self.id_set.union(id_set)
        self.interest_sum += interest_factor

if __name__ == "__main__":
    input_file = input("Enter input file: ")
    soln = Solution(input_file)
    soln.load_data()

